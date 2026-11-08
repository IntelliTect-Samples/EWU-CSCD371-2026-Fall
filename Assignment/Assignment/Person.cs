using System;
using System.Collections.Generic;
using System.Text;

namespace Assignment;

public class Person : IPerson
{
    public string FirstName{ get; }
    public string LastName{ get; }
    public IAddress Address{ get; }
    public string EmailAddress { get; }
    public Person(String firstName, String lastName, String email, IAddress address) {
        FirstName =
            firstName ??
            throw new ArgumentNullException(nameof(firstName));
        LastName =
            lastName ??
            throw new ArgumentNullException(nameof(lastName));
        Address =
            address ??
            throw new ArgumentNullException(nameof(address));
        EmailAddress =
            email ??
            throw new ArgumentNullException(nameof(email));
    }
}
