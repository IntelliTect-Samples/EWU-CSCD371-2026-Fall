using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace CanHazFunny.Services;

public sealed class JokeService : IJokeBuilder
{
    private readonly HttpClient _http;
    public JokeService(HttpClient http)
    {
        ArgumentNullException.ThrowIfNull(http);
        _http = http;
    }

    public async Task<string> GetJokeAsync()
    {
        var joke = await _http.GetStringAsync(
            "https://geek-jokes.sameerkumar.website/api?format=text"
        );

        return joke ?? string.Empty;
    }
}