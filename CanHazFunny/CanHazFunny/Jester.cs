using System;
using System.Diagnostics;

namespace CanHazFunny;

public class Jester
{
    private IOutput Output { get; }
    private IJokeService JokeService { get; }

    public Jester(IOutput? output, IJokeService? jokeService)
    {
        Output = output ?? throw new ArgumentNullException(nameof(output));
        JokeService = jokeService ?? throw new ArgumentNullException(nameof(jokeService));
    }

    public void TellJoke()
    {
        TimeSpan maxTimeLimit = TimeSpan.FromSeconds(5);

        Stopwatch stopwatch = Stopwatch.StartNew();

        string joke = string.Empty;
        bool containsChuckNorrisJoke;

        containsChuckNorrisJoke = true;

        while (containsChuckNorrisJoke && stopwatch.Elapsed < maxTimeLimit)
        {
            try
            {
                joke = JokeService.GetJoke() ?? string.Empty;
            }
            catch (InvalidOperationException)
            {
                break;
            }

            containsChuckNorrisJoke = joke.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase);
        }

        stopwatch.Stop();

        if (!containsChuckNorrisJoke)
        {
            Output.Write(joke);
        }
    }
}

