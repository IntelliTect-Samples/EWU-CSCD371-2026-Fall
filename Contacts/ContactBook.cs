namespace Contacts;

public class ContactBook
{
    private HashSet<IContact> Contacts { get; } = [];

    public bool Add(IContact item)
    {
        ArgumentNullException.ThrowIfNull(item); // null contacts would crash List 
        return Contacts.Add(item);
    }

    public bool Remove(IContact item)
    {
        return Contacts.Remove(item);
    }

    public bool RemoveById(int id)
    {
        var contact = Contacts.FirstOrDefault(c => c.Id == id);
        return contact is not null && Contacts.Remove(contact);
    }

    public bool Contains(IContact item)
    {
        return Contacts.Contains(item);
    }
    public bool IsEmpty => Contacts.Count == 0;

    //public IEnumerable<IContact> GetAll() => Contacts;
    public IContact[] GetAll() => [.. Contacts];
}
