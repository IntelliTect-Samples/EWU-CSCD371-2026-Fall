using System;

namespace CanHazFunny;

public class Jester
{
    private IJokeOutput JokeOutput { get; }

    private IJokeService JokeService { get; }

    public Jester(IJokeOutput jokeOutput, IJokeService jokeService)
    {
        JokeOutput = jokeOutput ?? throw new ArgumentNullException(nameof(jokeOutput));
        JokeService = jokeService ?? throw new ArgumentNullException(nameof(jokeService));
    }

    public void TellJoke()
    {
        string joke;
        do
        {
            joke = JokeService.GetJoke();
        }
        while (joke.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase));

        JokeOutput.Write(joke);
    }
}
