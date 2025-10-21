namespace Logger;
using System;

public record Student : IEntity 
{
    /// <summary>
    /// Id is exposed implicitly to be part of the public API. 
    /// used to represent the students uniquely within the system.
    /// Immutable to ensure consistency.
    /// Fetching student records relies on this unique identifier.
    /// 
    /// Name is calculated based on FullName, exposed implicitly as part of the public API.
    /// 
    /// StudentNumber is implicit and immutable, representing a unique identifier for each student.
    /// This number is different from Id for different contexts.
    /// 
    /// FullName is used to calculate Name, encapsulating the student's full name details.
    /// You wouldnt want to hide a students name.
    /// </summary>
    public Guid Id { get; init; }
    public string? StudentNumber { get; init; }
    public FullName FullName { get; init; }

    public Student(Guid id, FullName fullName, string? studentNumber = null) 
    { 
        Id = id;
        FullName = fullName;
        StudentNumber = string.IsNullOrWhiteSpace(studentNumber) ? throw new ArgumentException($"'nameof(studentNumber)' cannot be null or whitespace", nameof(studentNumber)) : studentNumber;
    }

    public string Name => FullName.Middle is null ?
        $"{FullName.First} {FullName.Last}"
        : $"{FullName.First} {FullName.Middle} {FullName.Last}";

}
