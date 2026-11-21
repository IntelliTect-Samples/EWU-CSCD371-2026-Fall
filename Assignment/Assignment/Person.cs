using Assignment.Interfaces;

namespace Assignment;

public sealed class Person : IPerson
{
    public string FirstName { get; }

    public string LastName { get; }

    public string ZipCode { get; }

    public string EmailAddress { get; }

    public Person(string firstName, string lastName, string email, string zipCode)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        ArgumentException.ThrowIfNullOrWhiteSpace(zipCode, nameof(zipCode));

        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        ZipCode = zipCode.Trim();
        EmailAddress = email.Trim();
    }
}
