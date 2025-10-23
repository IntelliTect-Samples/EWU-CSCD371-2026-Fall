#nullable enable
namespace Logger;

/// <summary>
/// An immutable value that travels with person-like entities.
///Chosen as a record struct because:
/// -it's small, commonly copied, and benefits from value semantics
/// -default immutability is desired
///</summary>

public readonly record struct FullName (
    string First,
    string Last,
    string? Middle = null)
{ /// <summary>
  /// Displays as "Last, First M." if Middle is present. Else, "Last, First".
  /// </summary>
  
    public override string ToString() =>
        Middle is { Length: > 0}
            ? $"{Last}, {First} {Middle[0]}."
            : $"{Last}, {First}";
}
