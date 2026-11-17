using System;

namespace Assignment;

public class Person
{
    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string EmailAddress { get; }
    public Address Address { get; }

    public Person(int id, string firstName, string lastName,
        string emailAddress, Address address)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(address);

        Id = id;
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        Address = address;
    }
}