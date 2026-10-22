using System.Text.Json;

namespace Contacts;

public class ContactBook
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private HashSet<IContact> Entities { get; } = [];

    public bool Add(IContact item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return Entities.Add(item);
    }

    public bool Remove(IContact item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return Entities.Remove(item);
    }

    public bool Contains(IContact item)
    {
        ArgumentNullException.ThrowIfNull(item);

        return Entities.Contains(item);
    }

    public IContact[] GetAll() => Entities.ToArray();

    public static ContactBook LoadFromFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        try
        {
            using FileStream stream = File.OpenRead(filePath);
            IContact[]? contacts = JsonSerializer.Deserialize<IContact[]>(stream, JsonOptions);
            if (contacts is null)
            {
                throw new JsonException("The contacts file must contain a JSON array.");
            }

            ContactBook contactBook = new();
            foreach (IContact contact in contacts)
            {
                contactBook.Add(contact);
            }

            return contactBook;
        }
        catch (FileNotFoundException)
        {
            return new ContactBook();
        }
        catch (DirectoryNotFoundException)
        {
            return new ContactBook();
        }
    }

    public void SaveToFile(string filePath)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        IContact[] contacts = GetAll();

        string destinationPath = Path.GetFullPath(filePath);
        string temporaryPath = $"{destinationPath}.{Guid.NewGuid():N}.tmp";

        try
        {
            using (FileStream stream = new(
                temporaryPath, FileMode.CreateNew, FileAccess.Write))
            {
                JsonSerializer.Serialize(stream, contacts, JsonOptions);
            }

            File.Move(temporaryPath, destinationPath, overwrite: true);
        }
        finally
        {
            File.Delete(temporaryPath);
        }
    }
}
