namespace Contacts;

// I considered CLI-only validation, but that isn't very reusable.
public record class FullName
{
    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleName { get; }

    public FullName(string firstName, string lastName, string? middleName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);

        FirstName = firstName;
        LastName = lastName;
        MiddleName = string.IsNullOrWhiteSpace(middleName) ? null : middleName;
    }

    public override string ToString()
    {
        if (MiddleName is null)
        {
            return $"{FirstName} {LastName}";
        }
        else
        {
            return $"{FirstName} {MiddleName} {LastName}";
        }
    }
}
