namespace Contacts;

public interface IContact
{
    Guid Id { get; }
    FullName Name { get; }
    string Email { get; }
}
