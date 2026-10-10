using System;

namespace CanHazFunny;

/// <summary>
/// Represents a jester that tells jokes.
/// </summary>
public class Jester
{
    /// <summary>
    /// The joke service used to retrieve jokes.
    /// </summary>
    private readonly IJokeService _jokeService;

    /// <summary>
    /// The joke output used to display jokes.
    /// </summary>
    private readonly IJokeConsoleOutput _jokeOutput;

    /// <summary>
    /// Initializes a new instance of the <see cref="Jester"/> class.
    /// </summary>
    /// <param name="jokeService">The joke service used to retrieve jokes.</param>
    /// <param name="jokeOutput">The joke output used to display jokes.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="jokeService"/> or <paramref name="jokeOutput"/> is null.</exception>
    /// <remarks>
    /// The constructor checks for null arguments and throws an <see cref="ArgumentNullException"/> if either <paramref name="jokeService"/> or <paramref name="jokeOutput"/> is null.
    /// </remarks>
    public Jester(IJokeService jokeService, IJokeConsoleOutput jokeOutput)
    {
        if (jokeService == null || jokeOutput == null)
        {
            throw new ArgumentNullException(jokeService == null ? nameof(jokeService) : nameof(jokeOutput));
        }
        _jokeService = jokeService;
        _jokeOutput = jokeOutput;
    }

    /// <summary>
    /// Tells a joke by retrieving it from the joke service and outputting it to the console.
    /// </summary>
    /// <remarks>
    /// The method retrieves a joke from the joke service and checks if it contains any of the forbidden words: "Chuck", "Norris", or "Texas Ranger". If the joke contains any of these words, it will continue to retrieve jokes until an acceptable one is found. If an exception occurs while retrieving a joke, it will output an error message to the console.
    /// </remarks>
    public void TellJoke()
    {
        string joke;

        // Keep retrieving jokes until an acceptable one is found or an exception occurs
        do
        {
            try
            {
                joke = _jokeService.GetJoke();
            }
            catch (Exception e)
            {
                _jokeOutput.WriteLine($"Error: {e.Message}");
                return;
            }
        } // Check if the joke contains any of the forbidden words (case-insensitive)
        while (joke.Contains("Chuck", StringComparison.OrdinalIgnoreCase) 
            || joke.Contains("Norris", StringComparison.OrdinalIgnoreCase)
            || joke.Contains("Texas Ranger", StringComparison.OrdinalIgnoreCase));

        _jokeOutput.WriteLine(joke);
    }
}