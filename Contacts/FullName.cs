namespace Contacts;

/// <summary>
/// A person's name. First and last are required; middle is optional and is
/// normalized to <c>null</c> when it is missing or blank.
/// </summary>
public sealed record FullName
{
    public FullName(string first, string last, string? middle = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(first);
        ArgumentException.ThrowIfNullOrWhiteSpace(last);

        First = first.Trim();
        Last = last.Trim();
        Middle = string.IsNullOrWhiteSpace(middle) ? null : middle.Trim();
    }

    public string First { get; }

    public string? Middle { get; }

    public string Last { get; }

    public override string ToString() =>
        Middle is null ? $"{First} {Last}" : $"{First} {Middle} {Last}";
}
