using System;

namespace CanHazFunny;

public class ConsoleOutput : IOutput
{
    public void Write(string message)
    {
        Console.WriteLine(message);
    }
}