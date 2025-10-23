using System;
namespace Logger;
public record Employee : PersonBase
{
    /// <summary>
    /// Position is Implicit and immutable, representing the job title or role of the employee 
    /// within the organization. Its specific to the employee as not every Ientity has a job title
    /// </summary>
	public string? Position { get; init; }


    /// <summary>
    /// Constructor forwards ID and Fullname to base class which
    /// ensures consistent initialization logic
    /// Poisition parameter is validated
    /// </summary>
    public Employee(Guid id, FullName fullName, string? position = null) : base(id, fullName) { 
        
        Position = string.IsNullOrWhiteSpace(position) ? throw new ArgumentException($"'{nameof(position)}' cannot be null or whitespace", nameof(position)) : position;
    }
}
