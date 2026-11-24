namespace PeopleEnricher.Models;

public sealed record PersonWithLocation(
    string FirstName,
    string LastName,
    string EmailAddress,
    string ZipCode,
    string City,
    string State
);