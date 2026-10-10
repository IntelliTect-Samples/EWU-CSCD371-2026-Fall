using System;

namespace CanHazFunny;

public class Jester
{
    private readonly IJokeService _jokeService;
    private readonly IJokeConsoleOutput _jokeOutput;

    public Jester(IJokeService jokeService, IJokeConsoleOutput jokeOutput)
    {
        if (jokeService == null || jokeOutput == null)
        {
            throw new ArgumentNullException(jokeService == null ? nameof(jokeService) : nameof(jokeOutput));
        }
        _jokeService = jokeService;
        _jokeOutput = jokeOutput;
    }

    public void TellJoke()
    {
        string joke;
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
        }
        while (joke.Contains("Chuck Norris"));

        _jokeOutput.WriteLine(joke);
    }
}