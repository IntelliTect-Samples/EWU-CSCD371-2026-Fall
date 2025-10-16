using System;
using System.IO;

namespace CanHazFunny;


public class JokeOutput : IOutput
{

    public void Write(string joke)
    {
        if (string.IsNullOrWhiteSpace(joke))
        {
            throw new ArgumentException("Joke cannot be null or empty.", nameof(joke));
        }
        Console.WriteLine(joke);
    }
}
