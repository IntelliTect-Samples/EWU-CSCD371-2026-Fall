using System.Net;

namespace Assignment;

public sealed record RunSummary(int Written, IReadOnlyDictionary<string, int> RejectionCounts);
public sealed record Person(string FirstName, string LastName, string Email, string ZipCode);
public sealed record EnrichedPerson(Person Person, string City, string State);
public sealed record Rejection(string Row, string Reason);

public sealed class PeopleEnricherService(HttpClient http, int maxAttempts = 10, int maxConcurrency = 5)
{
    internal const string PeopleHeader = "FirstName,LastName,EmailAddress,ZipCode";
    internal const string OutputHeader = "FirstName,LastName,EmailAddress,ZipCode,City,State";

    private readonly HttpClient _http = http;

    private readonly int _maxConcurrency = maxConcurrency >= 1
        ? maxConcurrency
        : throw new ArgumentOutOfRangeException(nameof(maxConcurrency), maxConcurrency, "Must be at least 1.");

    public async Task<RunSummary> RunAsync(string user, string outputPath, CancellationToken cancellationToken)
    {
        string csv = await DownloadPeopleAsync(user, cancellationToken);    // retries; throws if it never succeeds

        var parsed = ParsePeople(csv).ToList();
        var people = parsed.Where(r => r.Person is not null).Select(r => r.Person!).ToList();
        var rejections = parsed.Where(r => r.Rejection is not null).Select(r => r.Rejection!).ToList();

        var lookups = await LookupAllAsync(people, cancellationToken);      // concurrent, limited

        var enriched = lookups.Where(l => l.Result is not null).Select(l => l.Result!).ToList();
        rejections.AddRange(lookups.Where(l => l.Rejection is not null).Select(l => l.Rejection!));

        await WriteCsvAsync(outputPath, enriched, cancellationToken);       // temp file then move

        var counts = rejections.GroupBy(r => r.Reason).ToDictionary(g => g.Key, g => g.Count());
        return new RunSummary(enriched.Count, counts);
    }

    private async Task<string> DownloadPeopleAsync(string user, CancellationToken ct)
    {
        string? previous = null;
        for (int i = 0; i <= maxAttempts; i++)
        {
            string csv = await DownloadPeopleOnceAsync(user, ct);
            if (csv == previous)
            {
                return csv;
            }

            previous = csv;
        }

        throw new InvalidOperationException("The people CSV never came back the same twice in a row.");
    }

    private async Task<string> DownloadPeopleOnceAsync(string user, CancellationToken ct)
    {
        (string? csv, Exception? lastError) = await RetryAsync(async token =>
        {
            string body = await _http.GetStringAsync($"/people/{Uri.EscapeDataString(user)}", token);
            return body.Split('\n')[0].Trim() == PeopleHeader ? body : null;
        }, ct);

        return csv ?? throw new InvalidOperationException("The people CSV could not be downloaded.", lastError);
    }

    private static IEnumerable<(Person? Person, Rejection? Rejection)> ParsePeople(string csv) =>
        csv.Split('\n')
            .Select(line => line.Trim())
            .Skip(1)
            .Where(line => line.Length > 0)
            .Select(ParseRow);

    private static (Person? Person, Rejection? Rejection) ParseRow(string line)
    {
        string[] fields = [.. line.Split(',').Select(f => f.Trim())];

        if (fields.Length != 4)
        {
            return (null, new Rejection(line, "Wrong number of columns"));
        }

        if (fields.Any(f => f.Length == 0))
        {
            return (null, new Rejection(line, "Empty field"));
        }

        if (!IsValidEmail(fields[2]))
        {
            return (null, new Rejection(line, "Invalid email address"));
        }

        if (!IsValidZip(fields[3]))
        {
            return (null, new Rejection(line, "Invalid ZIP code"));
        }

        return (new Person(fields[0], fields[1], fields[2], fields[3]), null);
    }

    private async Task<(EnrichedPerson? Result, Rejection? Rejection)[]> LookupAllAsync(
        IReadOnlyList<Person> people, CancellationToken ct)
    {
        using SemaphoreSlim gate = new(_maxConcurrency);

        IEnumerable<Task<(EnrichedPerson?, Rejection?)>> tasks = people.Select(async person =>
        {
            await gate.WaitAsync(ct);
            try
            {
                EnrichedPerson? result = await LookupAsync(person, ct);
                return result is null
                    ? (null, new Rejection($"{person.FirstName},{person.LastName},{person.Email},{person.ZipCode}",
                        "ZIP lookup failed or returned invalid data"))
                    : ((EnrichedPerson?)result, (Rejection?)null);
            }
            finally
            {
                gate.Release();
            }
        });

        return await Task.WhenAll(tasks);
    }

    private async Task<EnrichedPerson?> LookupAsync(Person person, CancellationToken ct)
    {
        (EnrichedPerson? enriched, _) = await RetryAsync(async token =>
        {
            string body = await _http.GetStringAsync($"/people/zip/{person.ZipCode}", token);
            (string City, string State)? location = ParseLocation(body);
            return location is { } loc
                ? new EnrichedPerson(person, loc.City, loc.State.ToUpperInvariant())
                : null;
        }, ct);

        return enriched;
    }

    private async Task<(T? Value, Exception? LastError)> RetryAsync<T>(Func<CancellationToken, Task<T?>> attempt, CancellationToken ct)
        where T : class
    {
        Exception? lastError = null;
        for (int i = 0; i < maxAttempts; i++)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                T? result = await attempt(ct);
                if (result is not null)
                {
                    return (result, null);
                }
            }
            catch (HttpRequestException e) when (e.StatusCode == HttpStatusCode.NotFound)
            {
                return (null, e);   // permanent: asking again won't change the answer
            }
            catch (HttpRequestException e)
            {
                lastError = e;      // transient (418, 5xx, connection reset); try again
            }
            catch (TaskCanceledException e) when (!ct.IsCancellationRequested)
            {
                lastError = e;      // request timeout (not caller cancellation); try again
            }
        }

        return (null, lastError);
    }


    private static async Task WriteCsvAsync(string outputPath, IReadOnlyList<EnrichedPerson> people, CancellationToken ct)
    {
        string tempPath = outputPath + ".tmp";
        IEnumerable<string> lines = new[] { OutputHeader }.Concat(people.Select(p => string.Join(',',
            new[] { p.Person.FirstName, p.Person.LastName, p.Person.Email, p.Person.ZipCode, p.City, p.State }
                .Select(Escape))));

        try
        {
            await File.WriteAllLinesAsync(tempPath, lines, ct);
            File.Move(tempPath, outputPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }

    private static string Escape(string field) =>
        field.AsSpan().IndexOfAny(',', '"') >= 0
            ? $"\"{field.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : field;

    // email: has an @ with text on both sides, and no spaces
    private static bool IsValidEmail(string email)
    {
        int at = email.IndexOf('@', StringComparison.Ordinal);
        return at > 0 && at < email.Length - 1 && !email.Contains(' ', StringComparison.Ordinal);
    }

    // zip: exactly 5 digits
    private static bool IsValidZip(string zip) =>
        zip.Length == 5 && zip.All(char.IsAsciiDigit);

    // location line: "CITY","ST" (non-empty city, 2-letter state)
    private static (string City, string State)? ParseLocation(string body)
    {
        string[] parts = body.Trim().Split(',');
        if (parts.Length != 2)
        {
            return null;
        }

        string city = parts[0].Trim('"');
        string state = parts[1].Trim('"');

        return city.Length > 0 && state.Length == 2 && state.All(char.IsAsciiLetter)
            ? (city, state)
            : null;
    }
}
