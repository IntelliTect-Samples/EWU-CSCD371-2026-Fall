using System.Text.Json.Serialization;

namespace Contacts;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Student), "student")]
[JsonDerivedType(typeof(Employee), "employee")]
public interface IContact
{
    Guid Id { get; }
    FullName Name { get; }
    IReadOnlyList<string> Emails { get; }
    string? PhoneNumber { get; }
    string? Address { get; }
}
