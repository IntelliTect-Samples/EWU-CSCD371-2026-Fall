using System;
using System.Collections.Generic;
using System.Text;

namespace CanHazFunny;

public class Jester
{
    private readonly IJokeOutput _output;
    private readonly IJokeService _jokeService;

    public Jester(IJokeOutput output, IJokeService jokeService)
    {
        _output = output ?? throw new ArgumentNullException(nameof(output));
        _jokeService = jokeService ?? throw new ArgumentNullException(nameof(jokeService));
    }

    public void TellJoke()
    {
        string joke = _jokeService.GetJoke();

        while (joke.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase)) // filter for chuck noris jokes
        {
            joke = _jokeService.GetJoke();
        }

        _output.WriteJoke(joke);
    }
}