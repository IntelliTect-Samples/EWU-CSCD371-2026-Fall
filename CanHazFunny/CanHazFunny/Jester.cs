using System;

namespace CanHazFunny;

public class Jester
{

    public IOutputService OutputService { get; }
    public IJokeService JokeService { get; }

    public Jester(IOutputService outputService, IJokeService jokeService)
    {
         ArgumentNullException.ThrowIfNull(outputService);
         ArgumentNullException.ThrowIfNull(jokeService);

           OutputService = outputService;
           JokeService = jokeService;
    }

    public void TellJoke()
    {
        string joke;

        do { 
            joke = JokeService.GetJoke();

        } while (joke.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase));

        OutputService.Write(joke);
    }
    
}
