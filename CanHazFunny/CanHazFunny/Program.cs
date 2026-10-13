using System;

namespace CanHazFunny;

class Program
{
    /// <summary>
    /// The main entry point of the application. This method initializes the JokeService and ConsoleOutputService, creates a Jester instance, and tells a joke by fetching it from the external API.
    /// </summary>
    /// <param name="args"></param>
    static void Main(string[] args)
    {
        Run(() => new JokeService(), () => new ConsoleOutputService());
    }

    internal static void Run(Func<IJokeService> jokeServiceFactory, Func<IOutputService> outputServiceFactory)
    {
        Run(jokeServiceFactory(), outputServiceFactory());
    }

    internal static void Run(IJokeService jokeService, IOutputService outputService)
    {
        Jester jester = new(jokeService, outputService);
        jester.TellJoke();
    }
}
