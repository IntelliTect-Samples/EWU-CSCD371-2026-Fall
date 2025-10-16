using System;
using System.Net.Http;
using System.Text.Json;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private HttpClient HttpClient { get; } = new();

    public string GetJoke()
    {
        string json = HttpClient.GetStringAsync("https://geek-jokes.sameerkumar.website/api?format=json").Result;

        if (string.IsNullOrWhiteSpace(json))
            throw new InvalidOperationException("No joke received from the API.");

        var parsedJson = JsonSerializer.Deserialize<JsonElement>(json);
        if (!parsedJson.TryGetProperty("joke", out JsonElement jokeElement))
            throw new InvalidOperationException("No 'joke' property found in the response.");

        string? joke = jokeElement.GetString();
        if (string.IsNullOrWhiteSpace(joke))
            throw new InvalidOperationException("Joke text is empty or null.");

        return joke;
    }
}