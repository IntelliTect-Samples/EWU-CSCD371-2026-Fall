using System;

namespace Assignment;

public record Person(
    string FirstName,
    string LastName,
    string EmailAddress,
    IAddress Address) : IPerson
{
    public string FirstName { get; } =
        FirstName ?? throw new ArgumentNullException(nameof(FirstName));

    public string LastName { get; } =
        LastName ?? throw new ArgumentNullException(nameof(LastName));

    public string EmailAddress { get; } =
        EmailAddress ?? throw new ArgumentNullException(nameof(EmailAddress));

    public IAddress Address { get; } =
        Address ?? throw new ArgumentNullException(nameof(Address));
}