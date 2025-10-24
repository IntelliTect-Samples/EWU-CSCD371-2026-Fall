using System;

namespace Logger;

// Abstract base class implementing IEntity
// Implements the interface implicitly since derived classes should expose these members publicly
public abstract record EntityBase : IEntity
{
    // Implemented implicitly - Id is a core identifier that should be publicly accessible
    // across all entity types without requiring casting to IEntity
    public Guid Id { get; init; }

    // Abstract property - forces derived classes to implement Name
    // Not implemented here because each entity type should determine how to represent its name
    public abstract string Name { get; }

    protected EntityBase(Guid id)
    {
        Id = id;
    }
}