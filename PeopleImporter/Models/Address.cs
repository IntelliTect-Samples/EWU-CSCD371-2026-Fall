using System;

namespace Assignment;

/// <summary>
/// Represents a physical address with street, city, state, and zip code.
/// </summary>
/// <remarks>
/// This class is immutable and thread-safe. Once an instance is created, its properties cannot be changed.
/// </remarks>
/// <seealso cref="IAddress" />
/// <example>
/// <code>
/// var address = new Address("123 Main St", "Anytown", "CA", "12345");
/// Console.WriteLine(address.StreetAddress); // Output: 123 Main St
/// </code>
public sealed class Address : IAddress
{
    public Address(
        string streetAddress,
        string city,
        string state,
        string zip)
    {
        ArgumentNullException.ThrowIfNull(streetAddress);
        ArgumentNullException.ThrowIfNull(city);
        ArgumentNullException.ThrowIfNull(state);
        ArgumentNullException.ThrowIfNull(zip);

        StreetAddress = streetAddress;
        City = city;
        State = state;
        Zip = zip;
    }

    public string StreetAddress { get; }

    public string City { get; }

    public string State { get; }

    public string Zip { get; }
}