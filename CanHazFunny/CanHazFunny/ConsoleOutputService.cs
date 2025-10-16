using System;

namespace CanHazFunny;

public class ConsoleOutputService : IOutputService
{
    public void WriteJoke(string joke)
    {
        Console.WriteLine(joke);
    }
}