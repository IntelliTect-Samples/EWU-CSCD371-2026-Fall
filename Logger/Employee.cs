namespace Logger;

public record Employee : Person
{
    public required string Position { get; init; }

    // Implicit implementation because Name getter should be publicly available via Employee Instance
    public override string Name => $"{base.Name}, {Position}";

    public virtual bool Equals(Employee? obj)
    {
        if (obj is not Employee otherEmployee)
        {
            return false;
        }

        return this.Position == otherEmployee.Position && this.FullName.Equals(otherEmployee.FullName);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FullName, Position);
    }
}