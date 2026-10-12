/*
Which of the following code snippets should replace the //TODO comment in the Extensions class to make the output be "Hello Kevin Bost"?

a)
extension(Person)
{
    public static Person CreateKevin() 
        => new Person { FirstName = "Kevin", LastName = "Bost" };
}

b)
extension(Person person)
{
    public Person CreateKevin() 
        => new Person { FirstName = "Kevin", LastName = "Bost" };
}

c)
extension(Person person)
{
    public Person CreateKevin(string firstName, string lastName) 
        => new Person { FirstName = firstName, LastName = lastName };
}

d) 
extension(Person)
{
    public Person CreateKevin() 
        => new Person { FirstName = "Kevin", LastName = "Bost" };
}

e)
extension
{
    public static Person CreateKevin() 
        => new Person { FirstName = "Kevin", LastName = "Bost" };
}

Correct answer: a

*/

// Note: Static extension method
Person p = Person.CreateKevin();
Console.WriteLine($"Hello {p.FirstName} {p.LastName}");

public class Person
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
}

public static class Extensions
{
    // TODO
}