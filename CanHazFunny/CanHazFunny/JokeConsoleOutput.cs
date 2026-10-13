using System; 

namespace CanHazFunny;

public class JokeConsoleOutput: IConsoleOutput
{
    public void WriteJoke( string joke)
    {
        ArgumentNullException.ThrowIfNullOrEmpty(joke);
        Console.WriteLine(joke);
    }
}