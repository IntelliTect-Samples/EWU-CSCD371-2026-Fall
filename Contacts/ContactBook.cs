namespace Contacts;

public class ContactBook
{
    private HashSet<IContact> Entities { get; } = [];

    public bool Add(IContact item)
    {
        return Entities.Add(item);
    }

    public void Remove(IContact item)
    {
        Entities.Remove(item);
    }

    public bool Contains(IContact item)
    {
        return Entities.Contains(item);
    }

    public IContact[] GetAll() => Entities.ToArray();
}
