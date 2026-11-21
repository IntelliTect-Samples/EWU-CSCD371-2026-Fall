using Assignment.Interfaces;

namespace Assignment;

public sealed class Person : IPerson
{
    public string FirstName { get; }

    public string LastName { get; }

    public IAddress Address { get; }

    public string EmailAddress { get; }

    public int ID { get; }

    public Person(int id, string firstName, string lastName, string email, IAddress address)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(id), id,
                "Person ID must be greater than zero.");
        }

        ArgumentException.ThrowIfNullOrWhiteSpace(firstName, nameof(firstName));
        ArgumentException.ThrowIfNullOrWhiteSpace(lastName, nameof(lastName));
        ArgumentException.ThrowIfNullOrWhiteSpace(email, nameof(email));
        ArgumentNullException.ThrowIfNull(address, nameof(address));

        ID = id;
        FirstName = firstName.Trim();
        LastName = lastName.Trim();
        Address = address;
        EmailAddress = email.Trim();
    }
}
