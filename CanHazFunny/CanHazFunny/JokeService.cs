using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Text.Json;
using System.Collections.Generic;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private HttpClient HttpClient { get; } = new();

    public string GetJoke()
    {
        string json = HttpClient.GetStringAsync("https://geek-jokes.sameerkumar.website/api?format=json").Result;
        Dictionary<string, string>? JokeDict = JsonSerializer.Deserialize<Dictionary<string, string>>(json);
        if (JokeDict is not null)
        {
            return JokeDict["joke"];
        }
        return string.Empty;
    }
}
