namespace Logger;

// The FullName record class is a reference type that compares by value, best of both worlds for this use case.
public record FullName
{
    public string First { get; init; }
    public string Middle { get; init; } = string.Empty;
    public string Last { get; init; }
    // These roperties are immutable after initialization, because name elements are not expected to change.

    public FullName(string first, string last, string? middle)
    {
        if (string.IsNullOrWhiteSpace(first))
        {
            throw new ArgumentException($"'{nameof(first)}' cannot be null or whitespace.", nameof(first));
        }

        if (string.IsNullOrWhiteSpace(last))
        {
            throw new ArgumentException($"'{nameof(last)}' cannot be null or whitespace.", nameof(last));
        }

        First = first;
        Last = last;
        Middle = string.IsNullOrWhiteSpace(middle) ? string.Empty : middle.Trim();
    }

    public override string ToString()
    {
        return string.IsNullOrWhiteSpace(Middle)
            ? $"{First} {Last}"
            : $"{First} {Middle} {Last}";
    }
}