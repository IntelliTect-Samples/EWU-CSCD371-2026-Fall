namespace PeopleEnricher;

/// <summary>A valid person parsed from the downloaded CSV.</summary>
public sealed record Person(string FirstName, string LastName, string EmailAddress, string ZipCode);
