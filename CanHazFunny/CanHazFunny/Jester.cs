using System;

namespace CanHazFunny;

public class Jester
{
    private readonly InterfaceJokeService joke;
    private readonly InterfaceConsoleOutput jokeOutput;

    public Jester(InterfaceJokeService jokeService, InterfaceConsoleOutput output)
    {
        //ArgumentNullException.ThrowIfNull(jokeService);
        //ArgumentNullException.ThrowIfNull(output);
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