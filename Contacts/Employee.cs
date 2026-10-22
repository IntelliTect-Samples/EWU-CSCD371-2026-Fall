namespace Contacts;

public record Employee(FullName Name, IReadOnlyList<string> Emails, string JobTitle, string? Phone = null)
    : Contact(Name, Emails, Phone)
{
    public string JobTitle { get; } = Required(JobTitle, nameof(JobTitle));
}
