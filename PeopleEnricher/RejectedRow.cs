namespace PeopleEnricher;

/// <summary>
/// Represents a CSV data row that was removed from processing.
///
/// Rows are rejected when the contain invalid data,
/// or when enrichment cannot be completed successfully.
/// Rejected rows are reported to the caller
/// so they are not silently discarded.
/// </summary>
/// <param name="LineNumber">
/// The original line number in the source CSV where the rejected row was found.
/// The header row is line 1.
/// </param>
/// <param name="RawData">
/// The original unmodified CSV row that was rejected.
/// </param>
/// <param name="Reason">
/// A human-readable explanation describing why the row was rejected.
/// </param>
public record RejectedRow(
    int LineNumber,
    string RawData,
    string Reason);