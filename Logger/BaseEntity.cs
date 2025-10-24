namespace Logger;

public abstract record class BaseEntity(Guid Id) : IEntity
{
    // Guid is implemented implicitly because we want BaseEntity to obviously
    // have the functionality of IEntity, due to this class be abstract
    public Guid Id { get; init; } = Id;
    // Name is implemented implicitly because we want inheriting classes to
    // Be the ones to decide if it is implemented explicitly of not and for
    // Base entity to have the obvious functionality of IEntity
    public abstract string Name { get; }
}