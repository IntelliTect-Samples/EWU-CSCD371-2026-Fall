using System;

namespace CanHazFunny;

public class Jester
{
    private readonly IJokeOutput _jokeOutput;
    private readonly IJokeService _jokeService;

    public Jester(IJokeOutput jokeOutput, IJokeService jokeService)
    {
        _jokeOutput = jokeOutput ?? throw new ArgumentNullException(nameof(jokeOutput));
        _jokeService = jokeService ?? throw new ArgumentNullException(nameof(jokeService));
    }

    public void TellJoke()
    {
        string joke;
        do
        {
            joke = _jokeService.GetJoke();
        } while (joke.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase));

        _jokeOutput.WriteJoke(joke);
    }
}
