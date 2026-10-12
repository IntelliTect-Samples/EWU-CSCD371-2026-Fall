/*
Which of the following code snippets should replace the //TODO comment in the Extensions class to make the output be "Hello Kevin Bost"?

a)
extension(Person person)
{
    public string GetFullName()
        => $"{person.FirstName} {person.LastName}";
}

b)
extension(Person person)
{
    public static string GetFullName(this Person person)
        => $"{person.FirstName} {person.LastName}";
}

c)
extension(Person person)
{
    public static string GetFullName(Person person)
        => $"{person.FirstName} {person.LastName}";
}

d) 
extension(Person)
{
    public string GetFullName()
        => $"{person.FirstName} {person.LastName}";
}

e)
extension
{
    public string GetFullName(Person person)
        => $"{person.FirstName} {person.LastName}";
}

Correct answer: a

*/

Person p = new();
Console.WriteLine($"Hello {p.GetFullName()}");

public class Person
{
    public string FirstName { get; } = "Kevin";
    public string LastName { get; } = "Bost";
}

public static class Extensions
{
    // TODO
}