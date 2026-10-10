
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private HttpClient HttpClient { get; }

    public JokeService() : this(new HttpClient())
    {
    }

    public JokeService(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
        HttpClient = httpClient;
    }

    public string GetJoke()
    {
        string? response = HttpClient.GetStringAsync(
            "https://geek-jokes.sameerkumar.website/api?format=json").Result;

        if (response == null)
        {
            throw new InvalidOperationException(
                "Joke service returned no response.");
        }

        JsonDocument document = JsonDocument.Parse(response);

        if (!document.RootElement.TryGetProperty(
            "joke", out JsonElement jokeElement) ||
            jokeElement.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException(
                "Joke service returned no joke.");
        }

        string? joke = jokeElement.GetString();

        if (joke == null)
        {
            throw new InvalidOperationException(
                nameof(JokeService) +
                " returned null after deserialization.");
        }

        return joke;
    }
}
