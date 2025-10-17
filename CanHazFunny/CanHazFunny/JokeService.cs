using System.Net.Http;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private readonly HttpClient _httpClient;
    private readonly string _endpoint;


    public JokeService(HttpClient? httpClient = null, string? endpoint = null)
    {
        _httpClient = httpClient ?? new HttpClient();
        _endpoint = endpoint ?? "https://geek-jokes.sameerkumar.website/api";
    }
    public string GetJoke() => _httpClient.GetStringAsync(_endpoint).Result;
   
}
