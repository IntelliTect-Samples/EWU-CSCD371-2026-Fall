using System;

namespace CanHazFunny;

/// <summary>
/// Represents a jester who tells jokes.
/// </summary>
public class Jester(IJokeService jokeService, IOutputService outputService)
{
    /// <summary>
    /// The joke service used to retrieve jokes.
    /// </summary>
    private readonly IJokeService _jokeService = jokeService ?? throw new ArgumentNullException(nameof(jokeService));
    

    /// <summary>
    /// The output service used to display jokes.
    /// </summary>
    private readonly IOutputService _outputService = outputService ?? throw new ArgumentNullException(nameof(outputService));

   
    /// <summary>
    /// Tells a joke retrieved from the joke service, ensuring that it does not contain "Chuck Norris".
    /// If the joke contains "Chuck Norris", it will retrieve another joke until a suitable one is found. The joke is then output using the output service.
    /// </summary>
    public void TellJoke()
    {
        string joke;
        do
        {
            joke = _jokeService.GetJoke();
        }
        while (joke.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase));

        _outputService.Output(joke);
    }
}