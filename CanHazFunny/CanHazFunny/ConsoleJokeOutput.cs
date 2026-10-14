using System;

namespace CanHazFunny;

public class ConsoleJokeOutput : IJokeOutput
{
    public void WriteJoke(string joke)
    {
        Console.WriteLine(joke);
    }
}