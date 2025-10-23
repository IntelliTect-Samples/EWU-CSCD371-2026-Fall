namespace Logger;

public record Employee : IPerson
{
    // Defined Id implicitly because if an employee has the same name as another, they are still different employees,
    // So there should be a unique identifier available to the system.
    public Guid Id { get; init; }
    public FullName FullName { get; init; }

    public Employee(Guid id, FullName fullname)
    {
        ArgumentNullException.ThrowIfNull(fullname, nameof(fullname));
        Id = id;
        FullName = fullname;
    }

    // Delegating the Name property to the FullName record's Name property
    // Implicitly defined because it makes sense the programmer can access the employee's full name easily.
    public string Name => FullName.Name;
}
