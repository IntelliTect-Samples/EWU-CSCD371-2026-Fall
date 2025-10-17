using System;

namespace CanHazFunny;

public class ConsoleOutputService : IOutputService
{
    public void Write(string message)
    {
        Console.WriteLine(message);
    }
}  

