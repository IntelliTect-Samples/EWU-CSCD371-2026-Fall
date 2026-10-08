Person kevin = new();

Console.WriteLine($"Hello {kevin.Name} ({kevin.Age})");

public class Person
{
    public int Age { get; private set; } = 42;
    public string? Name { get; init; }
}