namespace Logger;
/// <summary>
/// Represents a person entity with a full name. Inherits IEntity for Id and Name.
/// </summary>

public interface IPerson : IEntity
{
    /// <summary>
    /// The full name of the person. Implemented implicitly for simplicity and direct access.    
    /// </summary>    
    FullName FullName { get; }
}

