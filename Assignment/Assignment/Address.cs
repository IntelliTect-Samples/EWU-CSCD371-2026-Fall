using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment;

public sealed class Address : IAddress
{
    public Address(string streetAddress, string city, string state, string zip)
    {
        ArgumentException.ThrowIfNullOrEmpty(streetAddress);

        StreetAddress = streetAddress;
        ArgumentException.ThrowIfNullOrEmpty(city);

        City = city;
        ArgumentException.ThrowIfNullOrEmpty(state);

        State = state;
        ArgumentException.ThrowIfNullOrEmpty(zip);

        Zip = zip;
    }

    public string StreetAddress { get; }
    public string City { get; }
    public string State { get; }
    public string Zip { get; }
}

