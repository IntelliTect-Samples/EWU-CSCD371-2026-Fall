using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment;

public sealed class Person : IPerson
{
    public Person(string firstName, string lastName, string emailAddress, IAddress address)
    {
        ArgumentException.ThrowIfNullOrEmpty(firstName);

        FirstName = firstName;
        ArgumentException.ThrowIfNullOrEmpty(lastName);

        LastName = lastName;
        ArgumentException.ThrowIfNullOrEmpty(emailAddress);

        EmailAddress = emailAddress;
        Address = address ?? throw new ArgumentNullException(nameof(address));
    }

    public string FirstName { get; }
    public string LastName { get; }
    public IAddress Address { get; }
    public string EmailAddress { get; }
}
