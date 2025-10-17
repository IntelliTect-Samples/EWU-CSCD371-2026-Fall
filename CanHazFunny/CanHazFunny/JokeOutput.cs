using System;
namespace CanHazFunny;

public class JokeOutput : IOutput
{
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }
}
