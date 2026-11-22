namespace Assignment;

public sealed record Rejection(int Row, string Reason);

public sealed record ImportResult(int WrittenCount, IReadOnlyList<Rejection> Rejections)
{
    public IEnumerable<(string Reason, int Count)> CountsByReason =>
        Rejections.GroupBy(r => r.Reason).Select(g => (g.Key, g.Count()));
}

public sealed class PeopleImporter(PeopleApiClient apiClient)
{
    private const int MaxConcurrentRequests = 8;
    private const string OutputHeader = "FirstName,LastName,EmailAddress,ZipCode,City,State";

    public async Task<ImportResult> ImportAsync(
        string user, string outputPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        // 1. Download. Throws if the people CSV can't be downloaded; nothing is written then.
        string csv = await apiClient.GetPeopleCsvAsync(user, cancellationToken);

        // 2. Parse rows into valid people and rejections.
        var parsed = csv.ReplaceLineEndings("\n").Split('\n')
            .Select((line, i) => (Row: i + 1, Line: line))
            .Skip(1)
            .Where(r => !string.IsNullOrWhiteSpace(r.Line))
            .Select(r => (r.Row, Result: CsvParser.ParsePerson(r.Line)))
            .ToList();
        var people = parsed.Where(p => p.Result.Person is not null)
            .Select(p => (p.Row, Person: p.Result.Person!)).ToList();
        var rejections = parsed.Where(p => p.Result.Person is null)
            .Select(p => new Rejection(p.Row, p.Result.Reason!)).ToList();

        // 3. Look up ZIPs concurrently, at most MaxConcurrentRequests at a time.
        //    A plain loop keeps the HTTP calls out of LINQ.
        using var gate = new SemaphoreSlim(MaxConcurrentRequests);
        var tasks = new List<Task<Location?>>(people.Count);
        foreach (var (_, person) in people)
            tasks.Add(LookupAsync(person.ZipCode, gate, cancellationToken));
        Location?[] locations = await Task.WhenAll(tasks);

        // 4. Build enriched people; failed lookups become rejections.
        var lookups = people.Zip(locations, (p, loc) => (p.Row, p.Person, Location: loc)).ToList();
        EnrichedPerson[] enriched = [.. lookups
            .Where(x => x.Location is not null)
            .Select(x => new EnrichedPerson(x.Person.FirstName, x.Person.LastName,
                x.Person.EmailAddress, x.Person.ZipCode, x.Location!.City, x.Location.State))];
        rejections.AddRange(lookups
            .Where(x => x.Location is null)
            .Select(x => new Rejection(x.Row, "ZIP lookup failed or returned invalid data")));

        // 5. Write to a temp file, then move it into place. The temp file never survives.
        IEnumerable<string> lines = new[] { OutputHeader }.Concat(enriched.Select(ToCsvLine));
        string fullPath = Path.GetFullPath(outputPath);
        string temporaryPath = fullPath + "." + Guid.NewGuid().ToString("N") + ".tmp";
        try
        {
            await File.WriteAllLinesAsync(temporaryPath, lines, cancellationToken);
            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
                File.Delete(temporaryPath);
        }

        return new ImportResult(enriched.Length, rejections);
    }

    private async Task<Location?> LookupAsync(string zipCode, SemaphoreSlim gate, CancellationToken ct)
    {
        await gate.WaitAsync(ct);
        try
        {
            return await apiClient.GetLocationAsync(zipCode, ct);
        }
        catch (HttpRequestException)
        {
            return null;   // retries used up, or a 404: remove this person, keep the run going
        }
        finally
        {
            gate.Release();
        }
    }

    private static string ToCsvLine(EnrichedPerson person) =>
        string.Join(",", new[]
        {
            person.FirstName, person.LastName, person.EmailAddress,
            person.ZipCode, person.City, person.State
        }.Select(EscapeCsv));

    private static string EscapeCsv(string value) =>
        value.IndexOfAny([',', '"', '\n', '\r']) >= 0
            ? "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\""
            : value;
}
