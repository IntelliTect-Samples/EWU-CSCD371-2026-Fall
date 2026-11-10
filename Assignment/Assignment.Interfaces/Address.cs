using System;

namespace Assignment;

public record Address(
    string StreetAddress,
    string City,
    string State,
    string Zip) : IAddress
{
    public string StreetAddress { get; } =
        StreetAddress ?? throw new ArgumentNullException(nameof(StreetAddress));

    public string City { get; } =
        City ?? throw new ArgumentNullException(nameof(City));

    public string State { get; } =
        State ?? throw new ArgumentNullException(nameof(State));

    public string Zip { get; } =
        Zip ?? throw new ArgumentNullException(nameof(Zip));
}