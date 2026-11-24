namespace PeopleEnricher;

/// <summary>
/// Summarizes a rejection reason and its occurrence count.
/// </summary>
public sealed record RejectionReasonSummary(
    RejectionStage Stage,
    string Reason,
    int Count);

/// <summary>
/// Contains grouped rejection statistics and individual rejected records.
/// </summary>
public sealed record RejectionReport(
    int TotalRejected,
    IReadOnlyList<RejectionReasonSummary> ReasonSummaries,
    IReadOnlyList<RejectedPerson> Details);