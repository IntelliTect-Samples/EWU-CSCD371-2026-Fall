using System.Net.Http;
using System.Net.Http.Json;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private HttpClient HttpClient { get; } = new();
    private sealed record FunnyResult(string Joke);

    public string GetJoke()
    {
        var joke = HttpClient.GetFromJsonAsync<FunnyResult>("https://geek-jokes.sameerkumar.website/api?format=json").Result;
        return joke!.Joke;
    }

}
