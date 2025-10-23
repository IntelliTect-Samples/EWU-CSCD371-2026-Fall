#nullable enable
namespace Logger;

/// <summary>
/// Abstract base that implements IEntity implicitly.
///	-We DO NOT implement Name here so derived classes must implement it.
///	-We implement equality by Id so all entities are compared by their unique identifier (useful for storage).
/// </summary>

public abstract record class EntityBase : IEntity
{
    /// <inheritdoc />
    /// Implemented implicitly because Id is part of the public entity contract.
    public Guid Id { get; init; } = Guid.NewGuid();
    /// <inheritdoc />
    /// Not provided here -- derived classes must implement it.
    public abstract string Name { get; }
    /// <summary>
    /// Entities are equal if their unique identifiers are equal.
    /// </summary>
    /// <param name="other">The other entity to compare.</param>
    /// <returns>True if equal, false otherwise.</returns>
    public virtual bool Equals(EntityBase? other) =>
        other is not null && Id == other.Id;
    
    public override int GetHashCode() => Id.GetHashCode();
}
