using Logger.Entities;

namespace Logger.Entities.People;

public abstract record class Person : EntityBase
{
    public FullName FullName { get; init; }

    /// <summary>
    /// Implemented implicitly since Name is part of a Person's public identity
    /// </summary>
    public override string Name => FullName.ToString();

    public Person(FullName name)
    {
        ArgumentNullException.ThrowIfNull(name, nameof(name));
        FullName = name;
    }
}
