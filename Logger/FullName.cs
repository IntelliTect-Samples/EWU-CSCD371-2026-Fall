using System;

namespace Logger;

/// <summary>
/// Full name record. First and Last are required; Middle is optional (nullable).
/// Reference record: simple and avoids unexpected copying that can cause issues with memory.
/// Immutable by design: the values are set at creation and don't change.
/// </summary>
public sealed record FullName
{
    public string First { get; init; }
    public string Last { get; init; }
    public string? Middle { get; init; }

    public FullName(string first, string last, string? middle = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(first, nameof(first));
        ArgumentException.ThrowIfNullOrWhiteSpace(last, nameof(last));

        First = first.Trim();
        Last = last.Trim();
        Middle = middle is null ? null : middle.Trim();
    }

    // displaying using ternary to handle optional middle name
    public string Name => Middle is null ? $"{First} {Last}" : $"{First} {Middle} {Last}";
}

