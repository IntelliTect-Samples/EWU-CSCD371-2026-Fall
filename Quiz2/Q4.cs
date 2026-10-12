/*
Given the following IPerson interface, and Person class. 
Which of the following code snippets will output "Hello Kevin Bost"?

(Select all that apply)

a)
Person p = new Person { FirstName = "Kevin", LastName = "Bost" };
Console.WriteLine($"Hello {p.FirstName} {p.LastName}");

b)
IPerson p = new Person { FirstName = "Kevin", LastName = "Bost" };
Console.WriteLine($"Hello {p.FirstName} {p.LastName}");

c)
var p = new Person { FirstName = "Kevin", LastName = "Bost" };
Console.WriteLine($"Hello {p.FirstName} {p.LastName}");

d)
IPerson p = new IPerson { FirstName = "Kevin", LastName = "Bost" };
Console.WriteLine($"Hello {p.FirstName} {p.LastName}");

e)
var p = new IPerson { FirstName = "Kevin", LastName = "Bost" };
Console.WriteLine($"Hello {p.FirstName} {p.LastName}");


Correct answers: a, b, c
*/

public interface IPerson
{
    string FirstName { get; set; }
    string LastName { get; set; }
}

public class Person : IPerson
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}
