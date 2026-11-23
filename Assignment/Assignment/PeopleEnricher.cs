namespace Assignment9;

/// <summary>The result of a run: who made it into the output and who was removed (and why).</summary>
public sealed record EnrichmentReport(
    int RowsDownloaded,
    IReadOnlyList<EnrichedPerson> Enriched,
    IReadOnlyList<Rejection> Rejections)
{
    /// <summary>Rejections grouped by reason, most common first.</summary>
    public IReadOnlyList<RejectionCount> RejectionCounts =>
    [
        .. Rejections
            .GroupBy(rejection => rejection.Reason)
            .Select(group => new RejectionCount(group.Key, group.Count()))
            .OrderByDescending(count => count.Count)
            .ThenBy(count => count.Reason, StringComparer.Ordinal),
    ];

    /// <summary>The output CSV: the header followed by one line per enriched person.</summary>
    public IEnumerable<string> ToCsvLines() =>
        Enriched.Select(person => person.ToCsvLine()).Prepend(EnrichedPerson.CsvHeader);
}

/// <summary>
/// Downloads people, looks up each person's city and state concurrently (with a cap on requests in
/// flight), and writes the people that were fully enriched.
/// </summary>
public sealed class PeopleEnricher
{
    private readonly MayhemMarmotClient client;
    private readonly RetryPolicy retryPolicy;
    private readonly int maxConcurrentLookups;

    public PeopleEnricher(MayhemMarmotClient client, RetryPolicy retryPolicy, int maxConcurrentLookups)
    {
        ArgumentNullException.ThrowIfNull(client);
        ArgumentNullException.ThrowIfNull(retryPolicy);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxConcurrentLookups, 1);
        this.client = client;
        this.retryPolicy = retryPolicy;
        this.maxConcurrentLookups = maxConcurrentLookups;
    }

    /// <summary>
    /// Runs the whole pipeline and writes <paramref name="outputPath"/>.
    /// Throws if the people CSV cannot be downloaded; in that case no file is written.
    /// </summary>
    public async Task<EnrichmentReport> RunAsync(string user, string outputPath, CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        EnrichmentReport report = await EnrichAsync(user, cancellationToken).ConfigureAwait(false);
        await CsvFileWriter.WriteAtomicallyAsync(outputPath, report.ToCsvLines(), cancellationToken).ConfigureAwait(false);
        return report;
    }

    /// <summary>Downloads and enriches people without writing anything.</summary>
    public async Task<EnrichmentReport> EnrichAsync(string user, CancellationToken cancellationToken)
    {
        // No catch here: if the people CSV can't be downloaded after retrying, the run fails.
        PeopleParseResult downloaded = await retryPolicy
            .ExecuteAsync(token => client.GetPeopleAsync(user, token), cancellationToken)
            .ConfigureAwait(false);

        LookupResult[] lookups = await LookUpAllAsync(downloaded.People, cancellationToken).ConfigureAwait(false);

        List<EnrichedPerson> enriched =
        [
            .. lookups
                .Where(lookup => lookup.Location is not null)
                .Select(lookup => new EnrichedPerson(lookup.Person, lookup.Location!)),
        ];

        List<Rejection> rejections =
        [
            .. downloaded.Rejections.Concat(lookups.Select(lookup => lookup.Rejection).OfType<Rejection>()),
        ];

        return new EnrichmentReport(downloaded.People.Count + downloaded.Rejections.Count, enriched, rejections);
    }

    private async Task<LookupResult[]> LookUpAllAsync(IReadOnlyList<Person> people, CancellationToken cancellationToken)
    {
        using var throttle = new SemaphoreSlim(maxConcurrentLookups, maxConcurrentLookups);

        // Start every lookup (each waits for a slot), then wait for all of them. Task.WhenAll only
        // completes once every task has finished, so the semaphore is never disposed while in use.
        var lookups = new List<Task<LookupResult>>(people.Count);
        foreach (Person person in people)
        {
            lookups.Add(LookUpAsync(person, throttle, cancellationToken));
        }

        return await Task.WhenAll(lookups).ConfigureAwait(false);
    }

    private async Task<LookupResult> LookUpAsync(Person person, SemaphoreSlim throttle, CancellationToken cancellationToken)
    {
        await throttle.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            Location location = await retryPolicy
                .ExecuteAsync(token => client.GetLocationAsync(person.ZipCode, token), cancellationToken)
                .ConfigureAwait(false);
            return new LookupResult(person, location, null);
        }
        catch (Exception exception) when (exception is TimeoutException || RetryPolicy.IsTransient(exception))
        {
            // Retries are exhausted for this person: remove them and keep going.
            // Caller cancellation and unexpected exceptions are not caught.
            string reason = exception switch
            {
                InvalidResponseException => RejectionReasons.LookupInvalidData,
                TimeoutException => RejectionReasons.LookupTimedOut,
                _ => RejectionReasons.LookupHttpError,
            };
            return new LookupResult(person, null, new Rejection(person.Describe(), reason, exception.Message));
        }
        finally
        {
            throttle.Release();
        }
    }

    private sealed record LookupResult(Person Person, Location? Location, Rejection? Rejection);
}
