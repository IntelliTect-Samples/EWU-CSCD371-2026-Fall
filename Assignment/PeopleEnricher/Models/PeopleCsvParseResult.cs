namespace PeopleEnricher;

/// <summary>
/// Contains the valid and rejected records from a people CSV.
/// </summary>
public sealed record PeopleCsvParseResult(
    IReadOnlyList<Person> People,
    IReadOnlyList<RejectedPerson> Rejections);