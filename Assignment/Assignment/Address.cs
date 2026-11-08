using System;

namespace Assignment;

public class Address(string streetAddress, string city, string state, string zip) : IAddress
{
    public string StreetAddress { get; } = streetAddress ?? throw new ArgumentNullException(nameof(streetAddress));
    public string City { get; } = city ?? throw new ArgumentNullException(nameof(city));
    public string State { get; } = state ?? throw new ArgumentNullException(nameof(state));
    public string Zip { get; } = zip ?? throw new ArgumentNullException(nameof(zip));
}
