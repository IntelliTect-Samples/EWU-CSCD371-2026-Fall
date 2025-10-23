#nullable enable
namespace Logger;

/// <summary>
/// Common base for person-like entities.
/// Keeps FullName property.
/// Implements IEntity implicitly.
/// </summary>

public abstract record class PersonEntity : EntityBase
{
    public required FullName FullName { get; init; }
    /// <inheritdoc />
    public override string Name => FullName.ToString();

}
