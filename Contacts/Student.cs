namespace Contacts;

public sealed record Student(FullName Name, string Email, string? Major = null)
    : Contact(Name, Email);
