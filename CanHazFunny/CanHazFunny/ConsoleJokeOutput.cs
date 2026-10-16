using System;

namespace CanHazFunny;

public class ConsoleJokeOutput : IJokeOutput
{
    public void Write(string joke)
    {
        Console.WriteLine(joke);
    }
}
