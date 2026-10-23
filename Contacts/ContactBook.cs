namespace Contacts;

public class ContactBook
{
    private HashSet<IContact> Entities { get; } = [];

    /// <summary>
    /// Adds a contact. Returns <c>false</c> (and adds nothing) if an equal contact is already stored.
    /// </summary>
    public bool Add(IContact item)
    {
        ArgumentNullException.ThrowIfNull(item);
        return Entities.Add(item);
    }

    /// <summary>
    /// Removes the stored contact equal to <paramref name="item"/>, even if it is a different instance.
    /// Returns <c>false</c> if no equal contact is stored.
    /// </summary>
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
}
