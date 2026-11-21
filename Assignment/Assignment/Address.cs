using Assignment.Interfaces;

namespace Assignment;

public sealed class Address : IAddress
{
    public string StreetAddress { get; }

    public string City { get; }

    public string State { get; }

    public string Zip { get; }

    public Address(string streetAddress, string city, string state, string zip)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streetAddress, nameof(streetAddress));
        ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));
        ArgumentException.ThrowIfNullOrWhiteSpace(state, nameof(state));
        ArgumentException.ThrowIfNullOrWhiteSpace(zip, nameof(zip));

        StreetAddress = streetAddress.Trim();
        City = city.Trim();
        State = state.Trim();
        Zip = zip.Trim();
    }
}
