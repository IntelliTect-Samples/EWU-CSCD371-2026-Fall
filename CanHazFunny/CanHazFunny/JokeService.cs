using System.Net.Http;
using System.Text.Json;
using System;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private HttpClient HttpClient { get; } = new();

    public string GetJoke()
    {
        string? joke = HttpClient.GetStringAsync("https://geek-jokes.sameerkumar.website/api").Result;
        
        if (joke == null)
        {
            throw new InvalidOperationException("Joke service returned null");
        }

        joke = JsonSerializer.Deserialize<string>(joke);

        if (joke == null)
        {
            throw new InvalidOperationException(nameof(joke) + " is null after deserialization");
        }

        return joke;
    }
}
