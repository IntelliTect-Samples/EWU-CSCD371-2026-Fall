using Assignment.Interfaces;

namespace Assignment;

public class EnrichedPerson(IPerson person, string city, string state)
{
    public string FirstName { get; } = person.FirstName;
    public string LastName { get; } = person.LastName;
    public string EmailAddress { get; } = person.EmailAddress;
    public string ZipCode { get; } = person.ZipCode;
    public string City { get; } = city;
    public string State { get; } = state;

    public override string ToString()
    {
        return $"{FirstName},{LastName},{EmailAddress},{ZipCode},{City},{State}";
    }
}

