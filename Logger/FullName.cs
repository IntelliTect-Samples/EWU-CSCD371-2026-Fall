namespace Logger;

public readonly record struct FullName(string First, string Last, string? Middle);

/// <summary>
/// Represents a person's full name, including optional middle name.
/// 
/// This type is defined as a readonly record struct so as to behave as a value object:
/// - We chose a value type because FullName should represent identity by content, not by reference.
///   It supports semantic equality, safe copying, and avoids heap allocation.
/// - We made it immutable by using readonly, ensuring all fields are set at construction and cannot be changed.
///   This guarantees lifecycle safety, thread safety, and prevents accidental mutation.
///   
/// this also mean if we did want a "rename" feature, we would need to make a new copy using "with" and handle associated logic.
/// see FullNameTests for example.
/// 
/// </summary>
