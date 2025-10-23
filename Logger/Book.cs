#nullable enable
namespace Logger;

/// <summary>
/// Book entity with Title/Author.
/// Implements IEntity implicity so Id/Name can be reached directly.
/// Includes init only implementation to prevent mutation.
/// Name = Title.
/// </summary>

public sealed record class Book : EntityBase
{
    public required string Title { get; init; }
    public string? Author { get; init; }

    /// <inheritdoc />
    public override string Name => Title;

    public bool Equals(Book? other) =>
            other is not null && Id == other.Id;

    public override int GetHashCode() => Id.GetHashCode();
}
