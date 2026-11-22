namespace PeopleEnricher.Tests;

internal static class TestData
{
    internal static Address CreateAddress(
        string streetAddress = "defaultStreetAddress",
        string city = "defaultCity",
        string state = "defaultState",
        string zip = "defaultZip")
    {
        return new Address(
            streetAddress,
            city,
            state,
            zip);
    }
    
    internal static Person CreatePerson(
        int id = 1,
        string firstName = "defaultFirstName",
        string lastName = "defaultLastName",
        string emailAddress = "defaultEmailAddress",
        Address? address = null)
    {
        return new Person(
            id,
            firstName,
            lastName,
            emailAddress,
            address ?? CreateAddress());
    }
}