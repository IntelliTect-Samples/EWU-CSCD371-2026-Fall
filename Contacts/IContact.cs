using System.Text.Json.Serialization;

namespace Contacts;

// Json Polymorphic attributes are used to specify how the interface and its derived types should be serialized and deserialized.
// The TypeDiscriminatorPropertyName specifies the property name that will be used to indicate the type of the object during serialization.
// The JsonDerivedType attributes specify the derived types and their corresponding discriminator values.
[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(Student), "student")]
[JsonDerivedType(typeof(Employee), "employee")]

public interface IContact
{
    // Place members here.
    int ID { get; }
    FullName name { get; }
    IReadOnlyList<string> Emails { get; }

}
