using System; 

namespace CanHazFunny;

public class JokeConsoleOutput: InterfaceConsoleOutput
{
    public void WriteJoke( string joke)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(joke);
        Console.WriteLine(joke);
    }
}