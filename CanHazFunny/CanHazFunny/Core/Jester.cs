using System;
using System.Threading.Tasks;

using CanHazFunny.Output;
using CanHazFunny.Services;

namespace CanHazFunny.Core;

public sealed class Jester
{
    private readonly IJokeBuilder _jokeService;
    private readonly IJokeOutputStringer _output;

    public Jester(IJokeBuilder jokeService, IJokeOutputStringer output)
    {
        ArgumentNullException.ThrowIfNull(jokeService);
        ArgumentNullException.ThrowIfNull(output);

        _jokeService = jokeService;
        _output = output;
    }

    public async Task TellJoke()
    {
        string joke;

        do
        {
            joke = await _jokeService.GetJokeAsync();
        }
        while (joke.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase));

        _output.Write(joke);
    }
}

