namespace Contacts;

public interface IContact
{
    Guid Id { get; }

    FullName Name { get; }

    string Email { get; }

    /// <summary>A single-line, human-readable description for display.</summary>
    string Describe();
}
