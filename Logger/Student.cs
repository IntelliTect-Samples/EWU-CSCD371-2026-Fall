namespace Logger;
using System;

public record Student : PersonBase
{
    /// <summary>
    /// StudentNumber is implicit and immutable, representing a unique identifier for each student.
    /// This number is different from Id for different contexts.
    /// </summary>
    public string? StudentNumber { get; init; }

    /// <summary>
    /// Constructor fowards ID and Fullname to base class which
    /// provides it implicitly
    /// </summary>
    public Student(Guid id, FullName fullName, string? studentNumber = null) : base(id, fullName)
    { 
        StudentNumber = string.IsNullOrWhiteSpace(studentNumber) ? throw new ArgumentException($"'nameof(studentNumber)' cannot be null or whitespace", nameof(studentNumber)) : studentNumber;
    }


}
