using System;
using System.Diagnostics.CodeAnalysis; // ADDED: Required for ExcludeFromCodeCoverage
using System.Threading.Tasks; 

namespace CanHazFunny;

// ADDED: Excludes this untestable file from the coverage report (Fixes 0% coverage)
[ExcludeFromCodeCoverage] 
sealed class Program
{
    static void Main(string[] args)
    {
        //Ask if they wanna hear a joke
        Console.Clear();
        Console.Write("Wanna hear a joke? (y/n): ");

        //Get their answer
        string answer = Console.ReadLine() ?? "n";
        
        //If they say no, exit program gracefully
        if (!string.Equals(answer, "y", StringComparison.OrdinalIgnoreCase))
        {
            Console.WriteLine("Aww, come on! You're no fun!");
            System.Environment.Exit(0);
            return;
        }

        Console.Clear();
        // FIX: Changed ConsoleOutputService to ConsoleOutput (Consistency)
        new Jester(new ConsoleOutput(), new JokeService()).TellJoke();
        return;
    }
}