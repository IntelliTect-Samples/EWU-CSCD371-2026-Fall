using System;
namespace Logger;
public record Employee : PersonBase
{
    /// <summary>
    /// Id Implemented implicitly as Employee must expose Id as part of its public API.
    /// Immutable to ensure consistency.
    /// Unique identifier for each employee within the system.
    /// 
    /// FullName is used to calculate Name, encapsulating the employee's full name details.
    /// 
    /// Name is calculated from FullName, implicitly defined so that it can be accessed directly
    /// 
    /// Position is Implicit and immutable, representing the job title or role of the employee within the organization.
    /// 
    /// </summary>
	public string? Position { get; init; }


    public Employee(Guid id, FullName fullName, string? position = null) : base(id, fullName) { 
        
        Position = string.IsNullOrWhiteSpace(position) ? throw new ArgumentException($"'{nameof(position)}' cannot be null or whitespace", nameof(position)) : position;
    }
}
