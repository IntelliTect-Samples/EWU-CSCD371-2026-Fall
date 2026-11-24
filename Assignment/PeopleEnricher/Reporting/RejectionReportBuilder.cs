namespace PeopleEnricher;

/// <summary>
/// Builds rejection reports from enrichment results.
/// </summary>
public static class RejectionReportBuilder
{
    /// <summary>
    /// Groups rejected records by stage and reason.
    /// </summary>
    /// <param name="result">The completed enrichment result.</param>
    /// <returns>A report containing rejection counts and details.</returns>
    public static RejectionReport Build(EnrichmentResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        RejectedPerson[] details = result.Rejections.ToArray();

        RejectionReasonSummary[] summaries = details
            .GroupBy(rejection => new
            {
                rejection.Stage,
                rejection.Reason
            })
            .OrderBy(group => group.Key.Stage)
            .ThenBy(
                group => group.Key.Reason,
                StringComparer.Ordinal)
            .Select(group => new RejectionReasonSummary(
                group.Key.Stage,
                group.Key.Reason,
                group.Count()))
            .ToArray();

        return new RejectionReport(
            details.Length,
            Array.AsReadOnly(summaries),
            Array.AsReadOnly(details));
    }
}