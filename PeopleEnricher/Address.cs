namespace PeopleEnricher;

public record Address
{
    public Address(
        string streetAddress,
        string city,
        string state,
        string zip)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(streetAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(city);
        ArgumentException.ThrowIfNullOrWhiteSpace(state);
        ArgumentException.ThrowIfNullOrWhiteSpace(zip);
        
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