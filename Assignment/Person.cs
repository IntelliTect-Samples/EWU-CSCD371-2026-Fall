using System;

namespace Assignment;

/// <summary>
/// Represents a person with a first name, last name, address, and email address.
/// </summary>
/// <remarks>
/// This class is immutable and thread-safe. Once an instance is created, its properties cannot be changed.
/// </remarks>
/// <seealso cref="IPerson" />
/// <example>
/// <code>
/// var address = new Address("123 Main St", "Anytown", "CA", "12345");
/// var person = new Person("John", "Doe", address, "john.doe@example.com");
/// Console.WriteLine(person.FirstName); // Output: John
/// </code>
/// </example>
public sealed class Person : IPerson
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