namespace Logger.entities;

// Student class inherits the IEntity implementation from Person. So both interface members are inherited implicitly
// and are publicly accessible as part of the class's API.
public record class Student : Person
{ 
    public string GraduationYear { get; set; }

    public Student(FullName fullName, string graduationYear) : base(fullName)
    {
        GraduationYear = graduationYear;

        if (string.IsNullOrWhiteSpace(GraduationYear))
        {
            throw new ArgumentException("Graduation year cannot be null or empty.", nameof(graduationYear));
        }
    }

}
