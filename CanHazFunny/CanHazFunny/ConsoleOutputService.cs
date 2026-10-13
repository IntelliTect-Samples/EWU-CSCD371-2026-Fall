using System;

namespace CanHazFunny;

// Core 3: create an implementation of the output interface that writes the joke to the console
public class ConsoleOutputService : IOutputService
{
    public void Output(string message)
    {
        Console.WriteLine(message);
    }
}