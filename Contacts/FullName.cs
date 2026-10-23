namespace Contacts;

public sealed record FullName(string FirstName, string LastName, string? Middle = null)
{
    public override string ToString()
        => Middle is null ? $"{FirstName} {LastName}" : $"{FirstName} {Middle} {LastName}";
}