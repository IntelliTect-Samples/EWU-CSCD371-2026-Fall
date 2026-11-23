namespace Assignment;

public sealed record Person(
    string FirstName,
    string LastName,
    string EmailAddress,
    string ZipCode);

public sealed record EnrichedPerson(
    string FirstName,
    string LastName,
    string EmailAddress,
    string ZipCode,
    string City,
    string State);
