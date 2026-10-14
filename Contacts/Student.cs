namespace Contacts;

public record Student(FullName Name, string Email, string Major, string? Phone = null)
    : Contact(Name, Email, Phone)
{
    public string Major { get; } = Required(Major, nameof(Major));
}
