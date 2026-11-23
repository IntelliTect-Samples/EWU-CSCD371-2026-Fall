namespace PeopleEnricher;

public sealed record EnrichmentResult(
    string EnrichedCsv,
    IReadOnlyList<RejectedRow> RejectedRows);