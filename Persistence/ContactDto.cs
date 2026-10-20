using Contacts;

namespace Persistence;

public record ContactDto
(
    ContactId Id,
    string ContactType,
    string FirstName,
    string LastName,
    string? MiddleName,
    string[] Emails
);