using System.Text.Json;

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
        using JsonDocument document = JsonDocument.Parse(json);

        string joke = document.RootElement.GetProperty("joke").GetString()
            ?? throw new JsonException("API returned null joke");

        return joke;
    }
}
