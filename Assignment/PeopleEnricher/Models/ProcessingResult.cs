namespace PeopleEnricher.Models;

public sealed record ProcessingResult(
    IReadOnlyList<PersonWithLocation> CompletedPeople,
    IReadOnlyList<RejectedPerson> RejectedPeople
);