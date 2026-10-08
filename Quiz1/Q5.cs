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