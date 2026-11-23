namespace PeopleEnricher;

public record RejectedRow(
    int LineNumber,
    string RawData,
    string Reason);