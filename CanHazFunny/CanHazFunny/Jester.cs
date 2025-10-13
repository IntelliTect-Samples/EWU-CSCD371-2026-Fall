using System;

namespace CanHazFunny;

public class Jester
{
    private readonly IOutput _outputService;
    private readonly IJokeService _jokeService;

    public Jester(IOutput outputService, IJokeService jokeService)
    {
        _outputService = outputService ?? throw new ArgumentNullException(nameof(outputService));
        _jokeService = jokeService ?? throw new ArgumentNullException(nameof(jokeService));
    }

    public string GetJoke()
    {
        return _jokeService.GetJoke();
    }

    // FIX: Filtering logic moved here to make the test pass and improve separation of concerns
    public void TellJoke()
    {
        string joke;
        do
        {
            joke = _jokeService.GetJoke();
        } 
        while (joke.Contains("Chuck Norris")); // Loop until the joke is clean

        _outputService.WriteLine(joke);
    }
}