namespace Contacts;

public record FullName(string FirstName, string LastName, string? MiddleName = null)
{
    public string FirstName { get; } = Required(FirstName, nameof(FirstName));

    public string LastName { get; } = Required(LastName, nameof(LastName));

    public string? MiddleName { get; } = string.IsNullOrWhiteSpace(MiddleName) ? null : MiddleName.Trim();

    public override string ToString() =>
        MiddleName is null ? $"{FirstName} {LastName}" : $"{FirstName} {MiddleName} {LastName}";

    private static string Required(string value, string paramName)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value, paramName);
        return value.Trim();
    }
}
