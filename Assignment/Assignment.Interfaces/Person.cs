namespace Assignment;

public record Person(
    string FirstName,
    string LastName,
    string EmailAddress,
    IAddress Address) : IPerson;