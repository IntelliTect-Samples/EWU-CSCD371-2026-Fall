using System;

namespace Assignment;

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