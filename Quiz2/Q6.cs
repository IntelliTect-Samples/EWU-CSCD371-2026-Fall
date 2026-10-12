/*
Given the following IPerson interface, and Person class. 
Which of the following code snippets will output "Hello Kevin Bost"?

(Select all that apply)

a)
Person p = new Person { FirstName = "Kevin", LastName = "Bost" };
Console.WriteLine($"Hello {p.FullName}");

b)
IPerson p = new Person { FirstName = "Kevin", LastName = "Bost" };
Console.WriteLine($"Hello {p.FullName}");

c)
var p = new Person { FirstName = "Kevin", LastName = "Bost" };
Console.WriteLine($"Hello {p.FullName}");

d)
IPerson p = new IPerson { FirstName = "Kevin", LastName = "Bost" };
Console.WriteLine($"Hello {p.FullName}");

e)
var p = new IPerson { FirstName = "Kevin", LastName = "Bost" };
Console.WriteLine($"Hello {p.FullName}");

Correct answer: b

*/

public interface IPerson
{
    string FirstName { get; set; }
    string LastName { get; set; }

    string FullName { get; }
}

public class Person : IPerson
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    // Note: Explicit interface implementation
    string IPerson.FullName => $"{FirstName} {LastName}";
}
