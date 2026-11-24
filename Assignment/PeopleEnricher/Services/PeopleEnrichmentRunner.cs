namespace PeopleEnricher;

/// <summary>
/// Coordinates downloading, enriching, reporting, and writing people data.
/// </summary>
public sealed class PeopleEnrichmentRunner
{
    private readonly PeopleEnrichmentService _enrichmentService;
    private readonly AtomicCsvFileWriter _fileWriter;

    /// <summary>
    /// Creates a runner using the supplied services.
    /// </summary>
    public PeopleEnrichmentRunner(
        PeopleEnrichmentService enrichmentService,
        AtomicCsvFileWriter fileWriter)
    {
        ArgumentNullException.ThrowIfNull(enrichmentService);
        ArgumentNullException.ThrowIfNull(fileWriter);

        _enrichmentService = enrichmentService;
        _fileWriter = fileWriter;
    }

    /// <summary>
    /// Executes a complete people enrichment run.
    /// </summary>
    public async Task<EnrichmentRunResult> RunAsync(
        string user,
        string outputPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        cancellationToken.ThrowIfCancellationRequested();

        // Download and enrich before touching the output file.
        EnrichmentResult result = await _enrichmentService
            .EnrichAsync(user, cancellationToken)
            .ConfigureAwait(false);

        // Write only after the complete enrichment operation succeeds.
        await _fileWriter.WriteAsync(
            outputPath,
            result.People,
            cancellationToken).ConfigureAwait(false);

        RejectionReport report = RejectionReportBuilder.Build(result);

        int peopleReceived =
            result.People.Count + result.Rejections.Count;

        return new EnrichmentRunResult(
            peopleReceived,
            result.People.Count,
            report,
            Path.GetFullPath(outputPath));
    }
}