User user = new() { Name = "Kevin", Age = 20 };

if (user is { Age: >= 18, Name: var name })
{
    Console.WriteLine($"Hello {name}");
}
else
{
    Console.WriteLine("No match");
}

public class User
{
    public required string Name { get; init; }
    public int Age { get; init; }
}
