using System.Text.Json;
using System.Text.Json.Serialization;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private readonly HttpClient _httpClient;

    public JokeService(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        _httpClient = httpClient;
    }

    public string GetJoke()
    {
        string json = _httpClient.GetStringAsync("https://geek-jokes.sameerkumar.website/api?format=json").Result;
        JokeResponse? response = JsonSerializer.Deserialize<JokeResponse>(json);

        return response?.Joke
            ?? throw new JsonException("API returned null joke");
    }

    private sealed class JokeResponse
    {
        [JsonPropertyName("joke")]
        public string? Joke { get; init; }
    }
}
