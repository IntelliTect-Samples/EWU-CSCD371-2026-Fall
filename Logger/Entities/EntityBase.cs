namespace Logger.Entities;

/// <summary>
/// EntityBase implicitly implements IEntity because Id and Name 
/// should be part of the public API for all entities.
/// </summary>
public abstract record class EntityBase : IEntity
{
    /// <inheritdoc />
    /// <summary>
    /// Implemented implicitly since all entities should expose their Id publicly,
    /// and there's no need to hide it behind the interface.
    /// </summary>
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Left abstract so derived classes must define a human-readable name.
    /// Implemented implicitly since Name is part of each entity’s public identity.
    /// </summary>
    public abstract string Name { get; }
}
