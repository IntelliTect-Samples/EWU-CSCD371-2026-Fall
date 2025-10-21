using System;
namespace Logger;
public record Employee : EntityBase
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
	public Guid Id { get; init; }
	public FullName FullName { get; init; }
	public string? Position { get; init; }


    public Employee(Guid id, FullName fullName, string? position = null) { 
        Id = id;
        FullName = fullName;
        Position = string.IsNullOrWhiteSpace(position) ? throw new ArgumentException($"'{nameof(position)}' cannot be null or whitespace", nameof(position)) : position;
    }

    public override string Name => FullName.Middle is null ?
        $"{FullName.First} {FullName.Last}"
        : $"{FullName.First} {FullName.Middle} {FullName.Last}";

    //Modifying FullName to incorporate immutability??



}
