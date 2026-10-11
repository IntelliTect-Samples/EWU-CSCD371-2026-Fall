using System;

namespace CanHazFunny;

public class CJokeOutput : IJokeOutput
{
    public void OutputJoke(string joke)
    {
        Console.WriteLine(joke);
    }
}