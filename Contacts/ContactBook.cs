using System.Text.Json;
using static Contacts.ContactBase;

namespace Contacts;



public sealed record class FullName(string First, string Last, string? Middle) 
{
    public override string ToString()
        => Middle is null
            ? $"{First} {Last}"
            : $"{First} {Middle} {Last}";
}
public abstract record ContactBase(FullName Name) : IContact
{
    private static int _nextID = 1;

    public int ID { get; init; } = _nextID++;

    public IReadOnlyList<string> Emails { get; init; } = new List<string>();
    public FullName name { get; } = Name;
}
public sealed record class Student(FullName name, string Major, string grad_year, IReadOnlyList<String> emails) : ContactBase(name)
{
    public IReadOnlyList<String> Emails { get; set; } = emails;
    
}
public sealed record class Employee(FullName name, string Department, string Company_Name, IReadOnlyList<String> emails) : ContactBase(name)
{ 
   
    public IReadOnlyList<String> Emails { get; set; } = emails;
}


public class ContactBook
{
    private HashSet<IContact> Entities { get; } = [];

    private const string FilePath = "contacts.json";

    public void Load()
    {
        if (!File.Exists(FilePath))
            return;

        var json = File.ReadAllText(FilePath);
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
        File.WriteAllText(FilePath, json);
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
