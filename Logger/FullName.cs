namespace Logger;
public record struct FullName(string FirstName, string LastName, string MiddleName = "")
{
    // This is a value type because typically when you are passing FullName, you aren't changing it.
    // It is mutable because it is a valid use case for someone to change their name legally.
    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(MiddleName)
            ? $"{FirstName} {LastName}"
            : $"{FirstName} {MiddleName} {LastName}";
    }
}