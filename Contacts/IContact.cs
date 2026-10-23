namespace Contacts;

public interface IContact
{
    int Id { get; }
    FullName Name { get; }
    string ContactInfo { get; }
}
