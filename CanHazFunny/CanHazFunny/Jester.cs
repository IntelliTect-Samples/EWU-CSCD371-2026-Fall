using System;

namespace CanHazFunny;

public class Jester
{
    private readonly IJokeOutput _jokeOutput;
    private readonly IJokeService _jokeService;

    public Jester(IJokeOutput jokeOutput, IJokeService jokeService)
    {
        _jokeOutput = jokeOutput;


        _jokeService = jokeService;
            
    }


    public void TellJoke()
    {
        string joke;

        do
        {
            joke = _jokeService.GetJoke();
        }
        while(joke.Contains("Chuck Norris"));

        _jokeOutput.OutputJoke(joke);
    }
}