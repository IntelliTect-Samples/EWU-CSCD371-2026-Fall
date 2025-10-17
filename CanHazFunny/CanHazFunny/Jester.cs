using System;

namespace CanHazFunny;

public class Jester
{
    private readonly IOutput Output;
    private readonly IJokeService JokeService;
    public Jester(IOutput output, IJokeService jokeService)
    {
        ArgumentNullException.ThrowIfNull(output);
        ArgumentNullException.ThrowIfNull(jokeService);
        Output = output;
        JokeService = jokeService;
    }

    public void TellJoke()
    {
        bool isChuckNorris = false;
        string joke;
        do {
            joke = JokeService.GetJoke();
            ArgumentNullException.ThrowIfNullOrWhiteSpace(joke);
            isChuckNorris = joke.Contains("Chuck", StringComparison.OrdinalIgnoreCase) 
                || joke.Contains("Norris", StringComparison.OrdinalIgnoreCase);
        } while (isChuckNorris);
        Output.ShowOutput(joke);
    }
}
