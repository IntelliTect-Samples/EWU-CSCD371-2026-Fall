using System;

namespace CanHazFunny;

public class Jester
{
    private readonly IJokeService joke;
    private readonly IConsoleOutput jokeOutput;

    public Jester(IJokeService jokeService, IConsoleOutput output)
    {
        ArgumentNullException.ThrowIfNull(jokeService); //null check
        ArgumentNullException.ThrowIfNull(output); //null check
        joke = jokeService;
        jokeOutput = output;
    }

    public void TellJoke()
    {
        string newJoke = joke.GetJoke();
        //conditional statement to ignore/discard jokes containing chuck norris
        while (newJoke.Contains("chuck norris", StringComparison.OrdinalIgnoreCase))
        {
            newJoke = joke.GetJoke();
        }

        jokeOutput.WriteJoke(newJoke);
    }
}