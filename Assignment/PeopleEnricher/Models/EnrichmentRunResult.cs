namespace PeopleEnricher;

/// <summary>
/// Describes the outcome of a completed enrichment run.
/// </summary>
public sealed record EnrichmentRunResult(
    int PeopleReceived,
    int PeopleWritten,
    RejectionReport RejectionReport,
    string OutputPath);