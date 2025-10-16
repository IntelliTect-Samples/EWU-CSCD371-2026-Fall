using System;

namespace CanHazFunny;

public class Jester
{
    private IJokeService JokeServiceInterface { get;  }
    private IOutput OutputInterface { get;  }

    public Jester(IJokeService? jokeService, IOutput? output)
    {
        ArgumentNullException.ThrowIfNull(jokeService);
        ArgumentNullException.ThrowIfNull(output);
        JokeServiceInterface = jokeService;
        OutputInterface = output;
    }

    public void TellJoke()
    {
        string generatedJoke = string.Empty;
        do
        {
            generatedJoke = JokeServiceInterface.GetJoke();
        } while (generatedJoke.Contains("Chuck Norris"));

        OutputInterface.WriteLine(generatedJoke);
    }
}