using System;

namespace CanHazFunny;

public class JokeOutput : IJokeConsoleOutput
{
    public void WriteLine(string joke)
    {
        if (joke == null)
        {
            throw new ArgumentNullException(nameof(joke));
        }

        Console.WriteLine("\n" + joke + "\n");
    }
}