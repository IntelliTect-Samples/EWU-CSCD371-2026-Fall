namespace Contacts;

// I considered CLI-only validation, but that isn't very reusable.
public record FullName
{

    public string FirstName { get; }
    public string LastName { get; }
    public string? MiddleName { get; }

    public FullName(string FirstName, string LastName, string? MiddleName = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(FirstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(LastName);

        this.FirstName = FirstName;
        this.LastName = LastName;
        this.MiddleName = string.IsNullOrWhiteSpace(MiddleName) ? null : MiddleName;
    }

    public override string ToString()
    {
        if (MiddleName == null)
        {
            return $"{FirstName} {LastName}";
        }
        else
        {
            return $"{FirstName} {MiddleName} {LastName}";
        }
    }
}