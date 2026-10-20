using Contacts;

namespace Persistence;

public interface IDataService
{
    /// <summary>
    /// Saves the specified contacts to the data store.
    /// </summary>
    /// <param name="contacts">The contacts to save.</param>
    void Save(IEnumerable<IContact> contacts);

    /// <summary>
    /// Loads contacts from the data store.
    /// </summary>
    /// <returns>The contacts loaded from the data store, or an empty array if none are available.</returns>
    IContact[] Load();
}