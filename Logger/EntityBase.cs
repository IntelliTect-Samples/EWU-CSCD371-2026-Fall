using System;
using System.Diagnostics.CodeAnalysis;

namespace Logger;

// EntityBase implements ID and Name implicitly so that derived classes automatically inherit ID
// and Name is abstract implicitly fulfilling the interface contract while requiring derived classes to provide their own implementation.
public abstract record class EntityBase : IEntity
{
    public Guid Id { get; init; } = Guid.NewGuid();

    public abstract string Name { get; }
    protected EntityBase(Guid id)
    {
        Id = id;
    }
    protected EntityBase()
    {
    }
}