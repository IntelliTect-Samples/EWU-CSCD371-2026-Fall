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

    public bool Replace(IContact existing, IContact updated)
    {
        if (!Entities.Remove(existing))
        {
            return false;
        }

        if (Entities.Add(updated))
        {
            return true;
        }

        Entities.Add(existing);
        return false;
    }

    public bool Contains(IContact item)
    {
        return Entities.Contains(item);
    }

    public IContact[] GetAll() => Entities.ToArray();
}
