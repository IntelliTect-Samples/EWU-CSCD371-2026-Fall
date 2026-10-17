namespace Contacts;

public interface IContact
{
    // Place members here.
    int ID { get; }
    FullName name { get; }
    IReadOnlyList<string> Emails { get; }
    
}

