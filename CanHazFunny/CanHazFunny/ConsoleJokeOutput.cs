using System;
using System.Collections.Generic;
using System.Text;

namespace CanHazFunny;

public class ConsoleJokeOutput : IJokeOutput
{
    public void WriteJoke(string joke)
    {
        Console.WriteLine(joke);
    }
}