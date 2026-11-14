namespace PeopleImporter;

public interface IPerson
{
    int ID { get; }
    string FirstName { get; }
    string LastName { get; }
    IAddress Address { get; }

    string EmailAddress { get; }
}
