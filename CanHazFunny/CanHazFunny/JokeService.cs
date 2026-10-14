using System;
using System.Net.Http;
using System.Net.Http.Json;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private HttpClient HttpClient { get; } = new();
    public string GetJoke()
    {
        JokeResponse? response = HttpClient
            .GetFromJsonAsync<JokeResponse>("https://geek-jokes.sameerkumar.website/api?format=json")
            .GetAwaiter()
            .GetResult();


        return response?.Joke 
            ?? throw new InvalidOperationException("The joke service did not return a joke.");
    }
    private sealed record JokeResponse(string Joke);
}



