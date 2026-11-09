using System;

namespace Assignment;

public record Person : IPerson
{
    public Person(
        string firstName,
        string lastName,
        IAddress address,
        string emailAddress)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(address);
        ArgumentNullException.ThrowIfNull(emailAddress);
        
        FirstName = firstName;
        LastName = lastName;
        Address = address;
        EmailAddress = emailAddress;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public IAddress Address { get; }
    public string EmailAddress { get; }
}