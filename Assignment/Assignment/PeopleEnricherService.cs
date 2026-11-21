using System.Net;

namespace Assignment;

public sealed record RunSummary(int Written, IReadOnlyDictionary<string, int> RejectionCounts);
public sealed record Person(string FirstName, string LastName, string Email, string ZipCode);
public sealed record EnrichedPerson(Person Person, string City, string State);
public sealed record Rejection(string Row, string Reason);
internal sealed record Location(string City, string State);

/// <summary>
/// Downloads people, validates them, looks up each person's city and state by ZIP, and writes
/// a CSV of only the people that were fully and correctly enriched.
/// </summary>
/// <remarks>
/// <para>A person is invalid when any of these is true:</para>
/// <list type="bullet">
/// <item>the row does not have exactly 4 columns;</item>
/// <item>first name, last name or email is empty;</item>
/// <item>the email does not have text on both sides of an <c>@</c>, or contains a space;</item>
/// <item>the ZIP code is not exactly 5 digits;</item>
/// <item>the ZIP lookup never returns a <c>"City","ST"</c> line with a non-empty city and a 2-letter state.</item>
/// </list>
/// <para>
/// Invalid people are removed, never repaired, and reported in <see cref="RunSummary.RejectionCounts"/>.
/// One bad person never stops the run. If the people CSV itself cannot be downloaded, the run
/// throws and no output file is written.
/// </para>
/// </remarks>
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

        Location?[] locations = await LookupAllAsync(people, cancellationToken);      // concurrent, limited

        var lookups = people.Zip(locations, (person, location) => (Person: person, Location: location)).ToList();
        var enriched = lookups
            .Where(l => l.Location is not null)
            .Select(l => new EnrichedPerson(l.Person, l.Location!.City, l.Location!.State))
            .ToList();
        rejections.AddRange(lookups
            .Where(l => l.Location is null)
            .Select(l => new Rejection(
                $"{l.Person.FirstName},{l.Person.LastName},{l.Person.Email},{l.Person.ZipCode}",
                "ZIP lookup failed or returned invalid data")));

        await WriteCsvAsync(outputPath, enriched, cancellationToken);       // temp file then move

        var counts = rejections.GroupBy(r => r.Reason).ToDictionary(g => g.Key, g => g.Count());
        return new RunSummary(enriched.Count, counts);
    }

    private async Task<string> DownloadPeopleAsync(string user, CancellationToken ct)
    {
        // A good response is identical every time, but "invalid data" swaps random ZIPs for
        // fake ones that still look valid. So only trust the CSV once two downloads agree.
        // (One extra pass: comparing always takes at least two downloads.)
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

    private async Task<Location?[]> LookupAllAsync(List<Person> people, CancellationToken ct)
    {
        using SemaphoreSlim gate = new(_maxConcurrency);

        // Start every lookup at once; the semaphore limits how many are in flight. This is a plain
        // loop rather than Select so the HTTP calls stay out of LINQ queries.
        List<Task<Location?>> tasks = new(people.Count);
        foreach (Person person in people)
        {
            tasks.Add(LookupAsync(person.ZipCode, gate, ct));
        }

        return await Task.WhenAll(tasks);
    }

    private async Task<Location?> LookupAsync(string zipCode, SemaphoreSlim gate, CancellationToken ct)
    {
        await gate.WaitAsync(ct);
        try
        {
            (Location? location, _) = await RetryAsync(async token =>
                ParseLocation(await _http.GetStringAsync($"/people/zip/{zipCode}", token)), ct);
            return location;
        }
        finally
        {
            gate.Release();
        }
    }

    /// <summary>
    /// Runs <paramref name="attempt"/> until it yields a non-null value or attempts run out.
    /// A null result (invalid data), an HTTP error, or a request timeout counts as a failed attempt;
    /// a 404 is permanent and is not retried. Caller cancellation and any other exception propagate.
    /// Returns the value (or null) and the last exception caught, so callers can report why it failed.
    /// </summary>
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
    private static Location? ParseLocation(string body)
    {
        string[] parts = body.Trim().Split(',');
        if (parts.Length != 2)
        {
            return null;
        }

        string city = parts[0].Trim('"');
        string state = parts[1].Trim('"');

        return city.Length > 0 && state.Length == 2 && state.All(char.IsAsciiLetter)
            ? new Location(city, state.ToUpperInvariant())
            : null;
    }
}
