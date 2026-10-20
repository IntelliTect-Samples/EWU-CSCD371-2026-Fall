namespace Contacts;

public class ContactBook
{
    private HashSet<IContact> Entities { get; } = [];

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

    public IContact[] GetAll()
    {
        return Entities.ToArray();
    }
}