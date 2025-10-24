
using System;
using System.Security.Principal;

namespace Logger;


public record Employee : PersonEntity
{   
    public string Role { get; init; }
    
      public Employee(Guid id, FullName fullName, string email, string role) : base(id, fullName, email)
    {
        if(string.IsNullOrWhiteSpace(role))
            throw new ArgumentNullException(nameof(role),"Role cannot be null or whitespace");
        Role = role;
    }
}