namespace Logger;

/// <summary>
/// The First Middle and Last variables are defined as 'reference' types.
/// 
/// The record is immutable since it consists solely of immutable string fields.
/// </summary>
public record class FullName
{
    public string First { get; }
    public string? Middle { get; }
    public string Last { get; }

    public FullName(string firstName, string? middleName, string lastName)
    {
        First = firstName ?? throw new ArgumentNullException(nameof(firstName));
        Middle = string.IsNullOrWhiteSpace(middleName) ? null : middleName;
        Last = lastName ?? throw new ArgumentNullException(nameof(lastName));
    }

    public override string ToString()
    {
        // space-separated names, ignoring null or empty
        return string.Join(" ", new[] { First, Middle, Last }.Where(s => !string.IsNullOrWhiteSpace(s)));
    }
}
