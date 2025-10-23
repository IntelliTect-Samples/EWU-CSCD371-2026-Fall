namespace Logger;
/// <summary>
/// Interface: Id is a Guid with an init-only setter; Name is a string.
/// </summary>
public interface IEntity
{
    Guid Id { get; init; }
    string Name { get; }

}
