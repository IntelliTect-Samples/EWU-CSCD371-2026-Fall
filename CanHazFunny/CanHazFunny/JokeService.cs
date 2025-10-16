using System.Net.Http;
using System.Net.Http.Json;
using System.Text.RegularExpressions;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private HttpClient HttpClient { get; } = new();

    public string GetJoke()
    {
        JokeResponse? response = HttpClient.GetFromJsonAsync<JokeResponse>("https://geek-jokes.sameerkumar.website/api?format=json").Result;

        if (response is not null)
        {
            return Regex.Unescape(response.Joke);
        } else {
            throw new HttpRequestException();
        }
    }
}
