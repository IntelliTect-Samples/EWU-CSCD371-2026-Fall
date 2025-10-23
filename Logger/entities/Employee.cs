namespace Logger.entities;

// Employee class inherits the IEntity implementation from Person. So both interface members are inherited implicitly
// and are publicly accessible as part of the class's API.
public record class Employee : Person
{
    public string Position { get; init; }
    
    public Employee(FullName fullName, string position) : base(fullName)
    {
        Position = position;

        if (string.IsNullOrWhiteSpace(Position))
        {
            throw new ArgumentException("Position cannot be null or empty.", nameof(position));
        }
    }
}
