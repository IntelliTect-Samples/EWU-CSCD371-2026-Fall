Person kevin = Person.Create("Kevin", 42);

string greeting = $"Hello {kevin.Name}, {nameof(Person.Age)}: {kevin.Age}";
Console.WriteLine(greeting);

public class Person
{
    public int Age { get; set; } = 40;
    public string? Name { get; init; }  

    public static Person Create(string name, int age)
        => new Person() { Name = name, Age = age };
}