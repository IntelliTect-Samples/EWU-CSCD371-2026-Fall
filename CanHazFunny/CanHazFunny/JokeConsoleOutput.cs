using System;

namespace CanHazFunny;

/// <summary>
/// Represents a service that outputs jokes to the console.
/// </summary>
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