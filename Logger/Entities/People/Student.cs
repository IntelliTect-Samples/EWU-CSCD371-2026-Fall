namespace Logger.Entities.People;

public record class Student : Person
{
    public decimal Gpa { get; init; }

    public Student(FullName fullName, decimal gpa) : base(fullName)
    {
        if (gpa < 0 || gpa > 4) throw new ArgumentOutOfRangeException(nameof(gpa), "GPA must be between 0 and 4.0");
        Gpa = gpa;
    }
}
