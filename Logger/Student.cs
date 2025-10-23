namespace Logger;

public record Student : IPerson
{
    // Defined Id implicitly because if a student has the same name as another, they are still different students,
    // So there should be a unique identifier available to the system.
    public Guid Id { get; init; }
    public FullName FullName { get; init; }

    public Student(Guid id, FullName fullname)
    {
        ArgumentNullException.ThrowIfNull(fullname, nameof(fullname));
        Id = id;
        FullName = fullname;
    }

    // Delegating the Name property to the FullName record's Name property
    // Implicitly defined because it makes sense the programmer can access the student's full name easily.
    public string Name => FullName.Name;
}
