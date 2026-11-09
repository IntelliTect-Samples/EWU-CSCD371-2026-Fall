namespace Assignment;

public record Address(
    string StreetAddress,
    string City,
    string State,
    string Zip) : IAddress;