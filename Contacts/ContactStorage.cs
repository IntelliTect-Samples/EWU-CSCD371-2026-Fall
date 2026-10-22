using System.Text.Json;

namespace Contacts;

public static class ContactStorage
{
    private static readonly JsonSerializerOptions Options = new() { WriteIndented = true };

    public static void Save(ContactBook book, string path)
    {
        ArgumentNullException.ThrowIfNull(book);

        // Write to a temp file first so a failed write never truncates the existing contacts file.
        string tempPath = path + ".tmp";

        try
        {
            File.WriteAllText(tempPath, JsonSerializer.Serialize(book.GetAll(), Options));
            File.Move(tempPath, path, overwrite: true);
        }
        finally
        {
            File.Delete(tempPath);
        }
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
