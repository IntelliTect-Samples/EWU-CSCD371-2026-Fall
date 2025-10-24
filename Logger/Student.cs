using System;


namespace Logger;


public record Student : PersonEntity
{
      public Student(Guid id, FullName fullName, string email) : base(id, fullName, email)
    {
    }
    
}