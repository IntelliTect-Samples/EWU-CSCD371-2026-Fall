namespace Contacts;

public sealed record FullName 
{   
    public FullName(string firstName, string lastName)
        : this(firstName, null, lastName)
    {
    }

    public FullName(string firstName, string? middleName, string lastName)
    {
        FirstName = Required(firstName, nameof(firstName));
        MiddleName = string.IsNullOrWhiteSpace(middleName)
            ? null
            : middleName.Trim();
        LastName = Required(lastName, nameof(lastName));
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleName { get; }

    public override string ToString() => 
        MiddleName is null 
            ? $"{FirstName} {LastName}" 
            : $"{FirstName} {MiddleName} {LastName}";

    private static string Required(string value, string name)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new ArgumentException($"'{name}' cannot be null or whitespace.", name);
        }

        return value.Trim();
    }   
}

