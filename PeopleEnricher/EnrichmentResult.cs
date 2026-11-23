namespace PeopleEnricher;

/// <summary>
/// Represents the final outcome of a successful enrichment operation.
/// </summary>
/// <param name="EnrichedCsv">
/// The resulting CSV containing the enriched Header,
/// and all successfully enriched data rows. 
/// </param>
/// <param name="RejectedRows">
/// The collection of RejectedRow data that was rejected during processing.
/// </param>
public sealed record EnrichmentResult(
    string EnrichedCsv,
    IReadOnlyList<RejectedRow> RejectedRows);