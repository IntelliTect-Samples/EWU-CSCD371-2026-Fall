using System;

namespace CanHazFunny;

public class Jester
{
    private IJokeService _jokeService;
    private IJokeOutput _jokeOutput;

    public Jester(IJokeService jokeService, IJokeOutput jokeOutput)
    {
        _jokeService = jokeService ?? throw new ArgumentNullException(nameof(jokeService));
        _jokeOutput = jokeOutput ?? throw new ArgumentNullException(nameof(jokeOutput));
    }
    public void TellJoke()
    {
        string joke = _jokeService.GetJoke();

        while (joke.Contains("Chuck Norris"))
        {
            joke = _jokeService.GetJoke();
        }

        _jokeOutput.OutputJoke(joke);
    }

}