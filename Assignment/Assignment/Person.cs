using System;

namespace Assignment;

public class Person(string firstName, string lastName, IAddress address, string emailAddress) : IPerson
{
    public string FirstName { get; } = firstName ?? throw new ArgumentNullException(nameof(firstName));
    public string LastName { get; } = lastName ?? throw new ArgumentNullException(nameof(lastName));
    public IAddress Address { get; } = address ?? throw new ArgumentNullException(nameof(address));
    public string EmailAddress { get; } = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
}
