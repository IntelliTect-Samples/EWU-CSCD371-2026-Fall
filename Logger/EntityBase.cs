namespace Logger;

/// <summary>
/// Abstract base class for all entities. Implements IEntity implicitly to expose Id directly.
/// Does not implement Name — forces derived classes to provide their own logic.
/// </summary>

public abstract class EntityBase : IEntity
{
    /// <summary>
    /// Implemented implicitly to expose Id as part of the entity's public API.
    /// Init-only to enforce immutability post-construction.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary> 
    /// "Do not implement the Name property in this abstract class"
    /// "Do force any derived classes to provide an implementation for Name"
    /// </summary>
    public abstract string Name { get; set; }
}