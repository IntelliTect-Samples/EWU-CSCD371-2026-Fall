namespace Assignment;

public class Address : IAddress
{
    public Address(string streetAddress, string city, string state, string zip)
    {
        StreetAddress = streetAddress;
        City = city;
        State = state;
        Zip = zip;
    }
    
    // FIX: Changed 'set' to 'init' for immutability
    public string StreetAddress { get; init; } 
    public string City { get; init; }
    public string State { get; init; }
    public string Zip { get; init; }
}