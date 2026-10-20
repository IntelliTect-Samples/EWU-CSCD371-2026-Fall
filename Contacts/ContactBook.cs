namespace Contacts;


/// <summary>
/// This class represents a contact book that can store and manage contacts implementing the IContact interface.
/// </summary>
public class ContactBook
{
    /// <summary>
    /// This property holds a collection of contacts in the contact book. It is implemented as a HashSet to ensure that each contact is unique and to provide efficient operations for adding, removing, and checking for the existence of contacts.
    /// </summary>
    private HashSet<IContact> Entities { get; } = [];


    /// <summary>
    /// Adds a contact to the contact book and updates the next ID for its type.
    /// Equal contacts are not added more than once.
    /// </summary>
    /// <param name="item">The contact to add.</param>
    public void Add(IContact item)
    {
        Entities.Add(item);

        string contactType = item.GetType().Name;
        int itemIdNumber = item.Id.Number;
        
        // if we have no value for this prefix, make one.
        // or the value for this prefix is lower than this items id + 1
        if (!_nextIds.TryGetValue(contactType, out int currentNext) || currentNext <= itemIdNumber)
            _nextIds[contactType] = itemIdNumber + 1;
    }


    /// <summary>
    /// Removes a contact from the contact book.
    /// </summary>
    /// <param name="item">The contact to remove.</param>
    public void Remove(IContact item)
    {
        Entities.Remove(item);
    }


    /// <summary>
    /// Determines whether the contact book contains a contact equal to the specified contact.
    /// </summary>
    /// <param name="item">The contact to locate.</param>
    /// <returns><see langword="true"/> if an equal contact is present; otherwise, <see langword="false"/>.</returns>
    public bool Contains(IContact item)
    {
        return Entities.Contains(item);
    }


    /// <summary>
    /// Gets a snapshot of all contacts currently stored in the contact book.
    /// </summary>
    /// <returns>An array containing the contacts in the contact book.</returns>
    public IContact[] GetAll() => Entities.ToArray();
    
    // dictionary keying a contact Type (Student, Employee, etc.) to its ID values
    private readonly Dictionary<string, int> _nextIds = [];

    /// <summary>
    /// Gets the next ID number for the specified contact type.
    /// </summary>
    /// <param name="contactType">The contact type whose next ID is requested.</param>
    /// <returns>The next ID number, or <c>1</c> if no contact of that type has been added.</returns>
    public int GetNextId(string contactType)
    {
        // returns value or if prefix not exist, default 1
        // NOTE: GetNextID does not mutate the dictionary, only actually adding the IContact item does
        // this just allows program to see what the next value for a prefix should be
        return _nextIds.GetValueOrDefault(contactType, 1);
    }
}




/// Notes:
/// --------------------------------------------------
/// 
/// 
/// - `ContactBook` must keep storing contacts in a `HashSet<IContact>` (a provided test checks this). ❌✔
/// - Adding a contact equal to a stored one must not add a duplicate, and the caller must be able to tell. ❌✔
/// - `Contains` and `Remove` must work with an equal but different instance. ❌✔
/// - Add whatever else your CLI needs (for example, search or list). ❌✔
///
/// 
/// The `HashSet` is what makes equality matter: it calls `Equals` and `GetHashCode` through `IContact`, so a wrong implementation shows up as duplicate or "missing" contacts.