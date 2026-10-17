using System.Text.Json;

namespace Contacts;

public sealed record class FullName(string First, string Last, string? Middle)
{
    public override string ToString()
        => Middle is null
            ? $"{First} {Last}"
            : $"{First} {Middle} {Last}";
}

public abstract record ContactBase : IContact
{
    private static int _nextID = 1;

    protected ContactBase(FullName name, IReadOnlyList<string> emails)
    {
        this.name = name;
        Emails = Array.AsReadOnly(emails.ToArray());
    }

    public int ID { get; init; } = _nextID++;

    public FullName name { get; init; }

    public IReadOnlyList<string> Emails { get; init; }

    public virtual bool Equals(ContactBase? other) =>
        other is not null &&
        GetType() == other.GetType() &&
        name == other.name &&
        Emails.SequenceEqual(other.Emails, StringComparer.Ordinal);

    public override int GetHashCode()
    {
        var hash = new HashCode();
        hash.Add(GetType());
        hash.Add(name);

        foreach (var email in Emails)
        {
            hash.Add(email, StringComparer.Ordinal);
        }

        return hash.ToHashCode();
    }
}

public sealed record Student : ContactBase
{
    public Student(
        FullName name,
        string major,
        string graduationYear,
        IReadOnlyList<string> emails) : base(name, emails)
    {
        Major = major;
        GraduationYear = graduationYear;
    }

    public string Major { get; init; }

    public string GraduationYear { get; init; }
}

public sealed record Employee : ContactBase
{
    public Employee(
        FullName name,
        string department,
        string companyName,
        IReadOnlyList<string> emails)
        : base(name, emails)
    {
        Department = department;
        CompanyName = companyName;
    }

    public string Department { get; init; }

    public string CompanyName { get; init; }
}

public class ContactBook
{
    private HashSet<IContact> Entities { get; } = [];

    private readonly string _filePath;

    public ContactBook(string filePath = "contacts.json")
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);
        _filePath = Path.GetFullPath(filePath);
    }

    public void Load()
    {
        if (!File.Exists(_filePath))
            return;

        var json = File.ReadAllText(_filePath);
        var items = JsonSerializer.Deserialize<IContact[]>(json, JsonOptionGiver.Options);

        if (items is not null)
        {
            foreach (var item in items)
                Entities.Add(item);
        }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(Entities.ToArray(), JsonOptionGiver.Options);
        File.WriteAllText(_filePath, json);
    }
    public bool Add(IContact item)
    {
        return Entities.Add(item);
    }

    public bool Remove(IContact item)
    {
        return Entities.Remove(item);
    }

    public bool Contains(IContact item)
    {

        return Entities.Contains(item);
    }

    public IContact[] GetAll() => Entities.ToArray();
}
