using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;
namespace CanHazFunny;

public class JokeService : IJokeService
{
    private HttpClient HttpClient { get; } = new();

    public string GetJoke()
    {
        string jsonJoke = HttpClient.GetStringAsync("https://geek-jokes.sameerkumar.website/api?format=json").Result;
        JokeResponse? jokeResponse = JsonSerializer.Deserialize<JokeResponse>(jsonJoke);

        return string.IsNullOrWhiteSpace(jokeResponse?.Joke)
            ? "No joke found."
            : jokeResponse.Joke;
    }

    internal sealed class JokeResponse 
    {
        [JsonPropertyName("joke")]
        public string? Joke { get; set; } 
    }
}
