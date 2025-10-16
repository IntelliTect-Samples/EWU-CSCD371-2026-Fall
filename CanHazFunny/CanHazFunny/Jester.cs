using System;

namespace CanHazFunny;

public class Jester
{
    private IJokeService JokeService { get; }
    private IOutputService OutputService { get; }
    
    public Jester(IJokeService jokeService, IOutputService outputService)
    {
        ArgumentNullException.ThrowIfNull(jokeService);
        ArgumentNullException.ThrowIfNull(outputService);
        JokeService = jokeService;
        OutputService = outputService;
    }

    public void TellJoke()
    {
        const int maxRetries = 50;
        int retries = 0;
        string joke = JokeService.GetJoke();
        while (joke.Contains("Chuck Norris") && retries < maxRetries)
        {
            joke = JokeService.GetJoke();
            retries++;
        }
        if (joke.Contains("Chuck Norris"))
        {
            OutputService.WriteJoke("Sorry, couldn't find a non-Chuck Norris joke after 50 attempts.");
        }
        else
        {
            OutputService.WriteJoke(joke);
        }
    }
}