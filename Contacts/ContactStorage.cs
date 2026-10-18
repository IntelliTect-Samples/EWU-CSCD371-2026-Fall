using System.Text.Json;

namespace Contacts;

public static class ContactStorage
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static void Save(ContactBook book, string path)
    {
        ArgumentNullException.ThrowIfNull(book);

        File.WriteAllText(path, JsonSerializer.Serialize(book.GetAll(), Options));
    }

    public static ContactBook Load(string path)
    {
        ContactBook book = new();

        if (!File.Exists(path))
        {
            return book;
        }

        IContact?[] contacts = JsonSerializer.Deserialize<IContact?[]>(File.ReadAllText(path), Options) ?? [];

        foreach (IContact? contact in contacts)
        {
            if (contact is not null)
            {
                book.Add(contact);
            }
        }

        return book;
    }
}
