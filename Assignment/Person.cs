
namespace Assignment;

public class Person : IPerson
{
    public Person(string firstName, string lastName, IAddress address, string email)
    {
        FirstName = firstName;
        LastName = lastName;
        EmailAddress = email;
        
        // DID YOU FORGET THIS LINE?
        Address = address; 
    }

    public string FirstName { get; }
    public string LastName { get; }
    public string EmailAddress { get; }
    public IAddress Address { get; } // If this is null, your Sort function crashes.
}