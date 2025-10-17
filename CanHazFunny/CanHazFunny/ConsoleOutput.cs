using System;

namespace CanHazFunny;

public class ConsoleOutput : IOutput
{
    public void ShowOutput(string message)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(message);
        Console.WriteLine(message);
    }
}

