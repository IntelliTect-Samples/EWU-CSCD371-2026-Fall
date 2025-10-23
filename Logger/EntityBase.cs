using System;

namespace Logger;

/// <summary>
/// Abstract base that implements IEntity.Id but leaves Name to be provided by derived classes.
/// </summary>
public abstract class EntityBase : IEntity
{
    public Guid Id { get; init; }

    // Do not implement Name here. Make derived classes to provide it.
    public abstract string Name { get; }
}
