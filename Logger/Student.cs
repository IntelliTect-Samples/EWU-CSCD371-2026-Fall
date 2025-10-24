namespace Logger;

public record Student : Person
{
    public required string StudentId { get; init; }

    // Implicit implementation because Name getter should be publicly available via Student Instance
    public override string Name => $"{base.Name} (ID: {StudentId})";

    public virtual bool Equals(Student? obj)
    {
        if (obj is not Student otherStudent)
        {
            return false;
        }

        return this.StudentId == otherStudent.StudentId && this.FullName.Equals(otherStudent.FullName);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(FullName, StudentId);
    }
}