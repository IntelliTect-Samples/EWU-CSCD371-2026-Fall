using System;

namespace CanHazFunny;

public class Jester
{
    
    private readonly IOutputService _outputService;
    private readonly IJokeService _jokeService;

    public Jester(IOutputService outputService, IJokeService jokeService)
    {
        _outputService = outputService ?? throw new ArgumentNullException(nameof(outputService));
        _jokeService = jokeService ?? throw new ArgumentNullException(nameof(jokeService));
    }

    public void TellJoke()
    {
        string joke;

        do { 
            joke = _jokeService.GetJoke();

        } while (joke.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase));

        _outputService.Write(joke);
    }
    
}
