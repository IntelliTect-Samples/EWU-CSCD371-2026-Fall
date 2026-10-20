using System.Text.Json;
using Contacts;

namespace Persistence;

public class JsonDataService(string filePath) : IDataService
{
    /// <summary>
    /// Serializes the specified contacts and writes them to the configured JSON file.
    /// </summary>
    /// <param name="contacts">The contacts to save.</param>
    public void Save(IEnumerable<IContact> contacts)
    {
        List<ContactDto> dtos = contacts
            .Select(ToDto)
            .ToList();
        
        File.WriteAllText(filePath, JsonSerializer.Serialize(dtos));
    }

    /// <summary>
    /// Loads and deserializes contacts from the configured JSON file.
    /// </summary>
    /// <returns>The loaded contacts, or an empty array if the file does not exist or contains a JSON null value.</returns>
    public IContact[] Load()
    {
        if (!File.Exists(filePath))
            return [];
        
        string json = File.ReadAllText(filePath);
        
        List<ContactDto>? dtos = JsonSerializer.Deserialize<List<ContactDto>>(json);

        if (dtos == null)
            return [];
        
        return dtos
            .Select(ToContact)
            .ToArray();
    }

    /// <summary>
    /// Converts a contact to its persistence representation.
    /// </summary>
    /// <param name="contact">The contact to convert.</param>
    /// <returns>A data transfer object containing the contact's persisted values.</returns>
    private static ContactDto ToDto(IContact contact)
    {
        return new ContactDto(
            contact.Id,
            contact.GetType().Name,
            contact.Name.FirstName,
            contact.Name.LastName,
            contact.Name.MiddleName,
            contact.Emails.Emails.ToArray()
        );
    }
    
    /// <summary>
    /// Creates a contact from its persistence representation.
    /// </summary>
    /// <param name="dto">The data transfer object to convert.</param>
    /// <returns>The contact represented by <paramref name="dto"/>.</returns>
    /// <exception cref="InvalidOperationException">The data transfer object contains an unknown contact type.</exception>
    private static IContact ToContact(ContactDto dto)
    {
        // create a full name record
        FullName fullName = new(
            dto.FirstName,
            dto.LastName,
            dto.MiddleName
        );

        // create an email record
        ContactEmails emails = new(dto.Emails);

        // return IContact created with those records according to type
        return dto.ContactType switch
        {
            nameof(Student) => new Student(
                fullName,
                emails
            ) { Id = dto.Id },

            nameof(Employee) => new Employee(
                fullName,
                emails
            ) { Id = dto.Id },

            _ => throw new InvalidOperationException(
                $"Unknown contact type: '{dto.ContactType}'.")
        };
    }
}