using System;

namespace CanHazFunny;

public class ConsoleJokeOutput : IJokeOutput
{
    public void OutputJoke(string joke)
    {
        Console.WriteLine(joke);
    }
}

