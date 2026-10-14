namespace Contacts;

public record Employee(FullName Name, string Email, string JobTitle, string? Phone = null)
    : Contact(Name, Email, Phone)
{
    public string JobTitle { get; } = Required(JobTitle, nameof(JobTitle));
}
