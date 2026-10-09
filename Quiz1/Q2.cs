/*

a) "Hello  (42)"
b) "Hello kevin (42)"
c) "Hello {kevin.Name} ({kevin.Age})"
d) No output, an exception is thrown
e) The code does not compile

Correct: a

*/

Person kevin = new();

Console.WriteLine($"Hello {kevin.Name} ({kevin.Age})");

public class Person
{
    public int Age { get; private set; } = 42;
    public string? Name { get; init; }
}