/*

a) "You are 45 years old"
b) "You are 42 years old"
c) "You are 40 years old"
d) No output, an exception is thrown
e) The code does not compile

Correct: a

*/

Person kevin = new();
int age = kevin.Age;
kevin.ShiftAge(age, -3);
kevin.AddAge(5);

Console.WriteLine($"You are {kevin.Age} years old");


public class Person
{
    public int Age { get; private set; } = 40;

    public void AddAge(int age)
    {
        Age += age;
    }

    public void ShiftAge(int age, int amount)
    {
        age += amount;
    }
}