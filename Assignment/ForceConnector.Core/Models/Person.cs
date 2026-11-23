
namespace ForceConnector.Core.Models;

public sealed record class Person
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string EmailAddress { get; init; } = string.Empty;
    public string ZipCode { get; init; } = string.Empty;
}
