/*

a) "Hello Kevin, Age: (42)"
b) "Hello Kevin, Age: (40)"
c) "Hello Kevin, Person.Age: (42)"
d) No output, an exception is thrown
e) The code does not compile

Correct: a had been the intended correct answer, but while putting these in I had inadvertently included the parenthesis so I have credited everyone for this question.

*/

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