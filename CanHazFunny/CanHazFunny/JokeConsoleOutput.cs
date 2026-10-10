using System;

namespace CanHazFunny;

public class JokeOutput : IJokeConsoleOutput
{
    public void WriteLine(string joke)
    {
        Console.WriteLine(joke);
    }
}