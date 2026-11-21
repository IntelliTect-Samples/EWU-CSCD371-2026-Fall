namespace PeopleEnricher;

public sealed record RejectedPerson(
    RejectionStage Stage,
    int? LineNumber,
    string Subject,
    string Reason);

public enum RejectionStage
{
    PeopleCsv,
    ZipLookup
}