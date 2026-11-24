namespace PeopleEnricher;

public sealed record EnrichedPerson(
    string FirstName,
    string LastName,
    string EmailAddress,
    string ZipCode,
    string City,
    string State);