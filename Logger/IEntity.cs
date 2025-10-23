#nullable enable
namespace Logger;

/// <summary>
/// Contract for entities that can be logged.
/// </summary>
public interface IEntity
{

    /// <summary>
    /// Unique identifier for the entity. Init only so it is set at creation and never mutated.
    /// </summary>
    Guid Id { get; init; }

    /// <summary>
    /// A display name for the entity.
    /// </summary>
    string Name { get; }

}
