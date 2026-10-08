Person kevin = new()
{
    Age = 42,
    Name = "Kevin"
};

UpdatePerson(kevin, 45);
kevin.Name += " Bost";

Console.WriteLine($"Hello {kevin.Name} ({kevin.Age})");

void UpdatePerson(Person person, int age)
{
    person = new Person()
    {
        Name = person.Name,
        Age = age
    };
}

public class Person
{
    public int Age { get; init; } = 40;
    public required string? Name { get; init; }
}