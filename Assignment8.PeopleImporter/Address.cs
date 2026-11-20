namespace Assignment8.ResilientImporter;


/// <summary>
/// Class houses address structure for standardization 
/// </summary>
public sealed class Address
{
    /// <summary>
    /// Enforces address structure to ensure the address is in US postal harmonized fmt
    /// </summary>
    /// <param name="streetAddress"></param>
    /// <param name="city"></param>
    /// <param name="state"></param>
    /// <param name="zip"></param>
    public Address(string streetAddress, string city, string state, string zip)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streetAddress, nameof(streetAddress));
        ArgumentException.ThrowIfNullOrWhiteSpace(city, nameof(city));
        ArgumentException.ThrowIfNullOrWhiteSpace(state, nameof(state));
        ArgumentException.ThrowIfNullOrWhiteSpace(zip, nameof(zip));

        StreetAddress = streetAddress;
        City = city;
        State = state;
        Zip = zip;
    }

    /// <summary>
    /// Getter setter blocks for address props
    /// </summary>
    public string StreetAddress { get; }

    public string City { get; }

    public string State { get; }

    public string Zip { get; }
}
