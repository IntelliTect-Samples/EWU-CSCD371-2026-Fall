using System.Net.Http;
using System.Text.Json;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private HttpClient HttpClient { get; } = new();

    public string GetJoke()
    {
        // Extra credit: API gets joke in json format
        var apiResponse = HttpClient.GetStringAsync("https://geek-jokes.sameerkumar.website/api?format=json").Result;
        using (var jokeObject = JsonDocument.Parse(apiResponse))
        {
            string joke = jokeObject.RootElement.GetProperty("joke").GetString() ?? "No joke received from api.";
            return joke;
        }
    }
}
