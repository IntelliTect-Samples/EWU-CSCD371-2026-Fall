namespace PeopleEnricher.Tests;

internal static class TestData
{
    internal static Person CreatePerson(
        string firstName = "defaultFirstName",
        string lastName = "defaultLastName",
        string emailAddress = "defaultEmailAddress",
        string zipCode = "defaultZipCode")
    {
        return new Person(
            firstName,
            lastName,
            emailAddress,
            zipCode: zipCode);
    }
}