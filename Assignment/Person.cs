using System;

namespace Assignment;

public sealed class Person : IPerson
{
    public Person(string firstName, string lastName, string emailAddress, IAddress address)
    {
        ArgumentNullException.ThrowIfNull(firstName);
        ArgumentNullException.ThrowIfNull(lastName);
        ArgumentNullException.ThrowIfNull(emailAddress);
        ArgumentNullException.ThrowIfNull(address);

        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        Address = address;
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string EmailAddress { get; }
    public IAddress Address { get; }
}
