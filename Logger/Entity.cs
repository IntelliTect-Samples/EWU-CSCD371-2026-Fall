namespace Logger;

// IEntity.Id is implemented explicitly to hide the identifier from the public API,
// while Name remains implicit so members can expose it directly.
public abstract record Entity : IEntity
{
    // Backing field for Id (not publicly visible)
    private readonly Guid _id = Guid.NewGuid();

    // Explicit implementation of IEntity.Id
    Guid IEntity.Id
    {
        get => _id;
        init {}
    }

    // Implicit implementation of IEntity.Name
    public abstract string Name { get; }
}