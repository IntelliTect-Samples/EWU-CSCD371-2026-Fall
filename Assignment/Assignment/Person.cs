namespace Assignment;

public class Person(int id, string firstName, string lastName, Address address, string emailAddress)
{
    public int Id { get; } = id;
    public string FirstName { get; } = firstName ?? throw new ArgumentNullException(nameof(firstName));
    public string LastName { get; } = lastName ?? throw new ArgumentNullException(nameof(lastName));
    public Address Address { get; } = address ?? throw new ArgumentNullException(nameof(address));
    public string EmailAddress { get; } = emailAddress ?? throw new ArgumentNullException(nameof(emailAddress));
}
