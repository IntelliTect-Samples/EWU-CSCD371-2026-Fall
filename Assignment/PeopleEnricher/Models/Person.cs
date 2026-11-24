namespace PeopleEnricher.Models;

public sealed record Person(
    string FirstName,
    string LastName,
    string EmailAddress,
    string ZipCode
);