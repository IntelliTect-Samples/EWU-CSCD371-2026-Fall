using System.Text.Json.Serialization;

namespace Contacts;

[JsonPolymorphic(TypeDiscriminatorPropertyName = "Type")]
[JsonDerivedType(typeof(Student), "Student")]
[JsonDerivedType(typeof(Employee), "Employee")]
public interface IContact
{
    Guid Id { get; }
    FullName Name { get; }
    IReadOnlyList<string> Emails { get; }
    string? Phone { get; }
}
