namespace PeopleEnricher;

public sealed record RowEnrichmentResult(
    string? EnrichedRow,
    RejectedRow? RejectedRow);