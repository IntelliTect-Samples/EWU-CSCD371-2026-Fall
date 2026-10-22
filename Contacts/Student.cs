namespace Contacts;

public record Student(FullName Name, IReadOnlyList<string> Emails, string Major, string? Phone = null)
    : Contact(Name, Emails, Phone)
{
    public string Major { get; } = Required(Major, nameof(Major));
}
