namespace Logger;
public interface IEntity
{
    // <summary>
    /// Implemented implicitly to expose Id as part of the entity's public API.
    /// Init-only to enforce immutability post-construction.
    /// </summary>
    Guid Id { get; init; }

    /// <summary>
    /// Implemented implicitly to allow direct access to Name from entity instances.
    /// Mutable to support domain workflows like renaming.
    /// </summary>
    string Name { get; }
}
