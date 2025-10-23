namespace Logger;

public abstract record Person : EntityBase
{
    public FullName FullName { get; init; }

    // This public member implicitly implements the IEntity interface and explicitly overrides the base class's Name property to define new logic.
    public override string Name => FullName.ToString();

    public Person(FullName fullName)
    {
        FullName = fullName;
    }
}
