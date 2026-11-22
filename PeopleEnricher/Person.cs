namespace PeopleEnricher;

public record Person
{
    public Person(
        int id,
        string firstName,
        string lastName,
        string emailAddress,
        Address address)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
        ArgumentException.ThrowIfNullOrWhiteSpace(firstName);
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName);
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);
        ArgumentNullException.ThrowIfNull(address);
        
        Id = id;
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = emailAddress;
        Address = address;
    }

    public int Id { get; }
    public string FirstName { get; }
    public string LastName { get; }
    public string EmailAddress { get; }
    public Address Address { get; }
}