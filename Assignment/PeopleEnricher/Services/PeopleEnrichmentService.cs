namespace PeopleEnricher;

/// <summary>
/// Enriches people with city and state information from ZIP lookups.
/// </summary>
public sealed class PeopleEnrichmentService
{
    private readonly IPeopleApiClient _apiClient;
    private readonly int _maxConcurrentLookups;

    /// <summary>
    /// Creates an enrichment service.
    /// </summary>
    /// <param name="apiClient">The API client.</param>
    /// <param name="maxConcurrentLookups">Maximum simultaneous ZIP requests.</param>
    public PeopleEnrichmentService(
        IPeopleApiClient apiClient,
        int maxConcurrentLookups = 8)
    {
        ArgumentNullException.ThrowIfNull(apiClient);

        if (maxConcurrentLookups < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxConcurrentLookups),
                "At least one concurrent lookup is required.");
        }

        _apiClient = apiClient;
        _maxConcurrentLookups = maxConcurrentLookups;
    }

    /// <summary>
    /// Downloads people and enriches each valid record.
    /// </summary>
    /// <param name="user">The user name or identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>Successfully enriched people and rejected records.</returns>
    /// <remarks>
    /// Failure to download the people feed aborts the entire operation.
    /// Individual ZIP lookup failures reject only the affected person.
    /// </remarks>
    public async Task<EnrichmentResult> EnrichAsync(
        string user,
        CancellationToken cancellationToken = default)
    {
        // A failure here aborts the entire operation.
        PeopleCsvParseResult feed = await _apiClient
            .GetPeopleAsync(user, cancellationToken)
            .ConfigureAwait(false);

        using SemaphoreSlim semaphore = new(
            _maxConcurrentLookups,
            _maxConcurrentLookups);

        Task<PersonOutcome>[] lookupTasks =
            new Task<PersonOutcome>[feed.People.Count];

        // Create the lookup tasks without performing HTTP calls inside LINQ.
        for (int index = 0; index < feed.People.Count; index++)
        {
            Person person = feed.People[index];

            lookupTasks[index] = EnrichPersonAsync(
                person,
                semaphore,
                cancellationToken);
        }

        // Task.WhenAll preserves the order of the supplied tasks.
        PersonOutcome[] outcomes = await Task.WhenAll(lookupTasks)
            .ConfigureAwait(false);

        EnrichedPerson[] enrichedPeople = outcomes
            .Where(outcome => outcome.Person is not null)
            .Select(outcome => outcome.Person!)
            .ToArray();

        RejectedPerson[] rejections = feed.Rejections
            .Concat(outcomes
                .Where(outcome => outcome.Rejection is not null)
                .Select(outcome => outcome.Rejection!))
            .ToArray();

        return new EnrichmentResult(
            Array.AsReadOnly(enrichedPeople),
            Array.AsReadOnly(rejections));
    }

    private async Task<PersonOutcome> EnrichPersonAsync(
        Person person,
        SemaphoreSlim semaphore,
        CancellationToken cancellationToken)
    {
        await semaphore.WaitAsync(cancellationToken)
            .ConfigureAwait(false);

        try
        {
            ZipLocation location;

            try
            {
                location = await _apiClient
                    .GetZipLocationAsync(person.ZipCode, cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException exception)
                when (!cancellationToken.IsCancellationRequested)
            {
                return Reject(
                    person,
                    $"ZIP lookup timed out after retries: {exception.Message}");
            }
            catch (InvalidDataException exception)
            {
                return Reject(person, exception.Message);
            }
            catch (HttpRequestException exception)
            {
                return Reject(
                    person,
                    $"ZIP lookup failed after retries: {exception.Message}");
            }
            catch (IOException exception)
            {
                return Reject(
                    person,
                    $"ZIP lookup failed after retries: {exception.Message}");
            }
            catch (TimeoutException exception)
            {
                return Reject(
                    person,
                    $"ZIP lookup timed out after retries: {exception.Message}");
            }

            EnrichedPerson enrichedPerson = new(
                person.FirstName,
                person.LastName,
                person.EmailAddress,
                person.ZipCode,
                location.City,
                location.State);

            return new PersonOutcome(enrichedPerson, null);
        }
        finally
        {
            semaphore.Release();
        }
    }

    private static PersonOutcome Reject(
        Person person,
        string reason)
    {
        RejectedPerson rejection = new(
            RejectionStage.ZipLookup,
            null,
            $"{person.FirstName} {person.LastName} (ZIP {person.ZipCode})",
            reason);

        return new PersonOutcome(null, rejection);
    }

    private sealed record PersonOutcome(
        EnrichedPerson? Person,
        RejectedPerson? Rejection);
}