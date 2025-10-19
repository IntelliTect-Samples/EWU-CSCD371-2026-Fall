using System;
namespace Logger;
public record Employee(Guid Id, FullName FullName, string Position) : IEntity
{
	/// <summary>
	/// Implemented implicitly as Employee must expose Id as part of its public API.
	/// Being a record, immutability is preserved through the constructor.
	/// </summary>
	public Guid Id { get; init; } = Id;
	/// <summary>
	/// Implemented implicity as a calculated property based on full name
	/// This ensures consistency if Full Name changes
	/// Name should be derived from FullName
	///</summary>
	public string Name => FullName.Middle is null ?
		$"{FullName.First} {FullName.Last}"
		: $"{FullName.First} {FullName.Middle} {FullName.Last}";
	/// <summary>
	/// This is implemented Explicity to honor the interface contract
	/// preversing from mutating Name directly
	/// </summary>
	string IEntity.Name
	{
		get => Name;
		set { }
    }

    ///<summary>
	///position is stored as a property with init-only setter so its part of the record
	///</summary>
    public string Position { get; init; } = Position;

}
