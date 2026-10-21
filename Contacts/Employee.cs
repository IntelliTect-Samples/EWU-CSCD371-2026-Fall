namespace Contacts;
 
public sealed record Employee(FullName Name, string Email, string? JobTitle = null)
    : Contact(Name, Email);
