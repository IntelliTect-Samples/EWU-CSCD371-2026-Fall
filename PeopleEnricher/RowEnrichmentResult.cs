namespace PeopleEnricher;

/// <summary>
/// Represents the outcome of attempting to enrich a valid CSV data row.
///
/// A row enrichment can result in one of two outcomes:
/// <list type="bullet">
/// <item>
/// <description>
/// The row was successfully enriched and <see cref="EnrichedRow"/>
/// contains the enriched CSV row while <see cref="RejectedRow"/>
/// is <see langword="null"/>
/// </description>
/// </item>
/// <item>
/// <description>
/// The row could not be enriched and <see cref="RejectedRow"/>
/// contains the rejected information while <see cref="EnrichedRow"/>
/// is <see langword="null"/>
/// </description>
/// </item>
/// </list>
/// Exactly one of <see cref="EnrichedRow"/> or <see cref="RejectedRow"/>
/// is expected to contain a value.
/// </summary>
/// <param name="EnrichedRow">
/// The enriched CSV row containing the appended City and State information.
/// This value is <see langword="null"/> when the row was rejected.
/// </param>
/// <param name="RejectedRow">
/// Information describing why a row was rejected.
/// This value is <see langword="null"/> when the row was successfully enriched.
/// </param>
public sealed record RowEnrichmentResult(
    string? EnrichedRow,
    RejectedRow? RejectedRow);