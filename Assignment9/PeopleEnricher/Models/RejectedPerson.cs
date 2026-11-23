
namespace PeopleEnricher.Models;

public sealed record RejectedPerson(
    int RowNumber,
    string Reason
);