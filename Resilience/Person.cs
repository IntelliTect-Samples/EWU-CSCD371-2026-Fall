using Resilience.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace Resilience;

public class Person : IPerson
{
    public string FirstName { get; }
    public string LastName { get; }
    public IAddress Address { get; }
    public string EmailAddress { get; }
    public int ID { get; }
    public Person(int id, String firstName, String lastName, String email, IAddress address)
    {
        if (id <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(id),
                id,
                "Person ID must be greater than zero.");
        }
        ID = id;
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