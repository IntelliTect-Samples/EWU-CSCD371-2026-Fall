namespace Logger;

// The FullName is defined as a record class to leverage records built in value based equality and immutability.
// Which is valuable for representing data objects like names where the focus is on the data itself rather than identity.
// The record is immutable because all of its properties have init-only setters.
public record class FullName
{
    public string First { get; init; }
    public string? Middle { get; init; }
    public string Last { get; init; }
    public FullName(string first, string? middle, string last)
    {
        First = first;
        Middle = middle;
        Last = last;
    }
    public override string ToString() =>
        string.Join(" ", new[] { First, Middle, Last }
            .Where(part => !string.IsNullOrWhiteSpace(part)));
}
