using Logger;
using System;

public record Student : IEntity 
{
    /// <summary>
    /// Id is exposed implicitly to be part of the public API. 
    /// used to represent the students uniquely within the system.
    /// Immutable to ensure consistency.
    /// 
    /// Name is calculated based on FullName, exposed implicitly as part of the public API.
    /// Ensures consistency if FullName changes.
    /// 
    /// StudentNumber is implicit and immutable, representing a unique identifier for each student.
    /// 
    /// FullName is used to calculate Name, encapsulating the student's full name details.
    /// </summary>
    public Guid Id { get; init; }
    public string? studentNumber { get; init; }
    public FullName FullName { get; init; }

    public string Name => FullName.Middle is null ?
        $"{FullName.First} {FullName.Last}"
        : $"{FullName.First} {FullName.Middle} {FullName.Last}";

}
