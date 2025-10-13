using System.Net.Http;
using System.Text.Json;
using System;

namespace CanHazFunny;

public class JokeService : IJokeService
{
    private HttpClient HttpClient { get; } = new();

    public string GetJoke()
    {
        try
        {
            // Calls the mockable method, which handles the network call
            string? joke = GetJokeFromApi(); 
            
            // Handles the branch where GetString() returns null (the final missing branch coverage)
            return joke ?? "No joke found"; 
        }
        // Handles network or JSON parsing errors
        catch (Exception ex) when (ex is HttpRequestException || ex is JsonException) 
        {
            return "Error retrieving joke. Please try again later";
        }
    }

    // NEW: This is the method the Mockable class in JesterTests.cs will now successfully override.
    protected virtual string? GetJokeFromApi() 
    {
        // This is the production code that makes the actual network call
        string json = HttpClient.GetStringAsync("https://geek-jokes.sameerkumar.website/api?format=json").Result;

        using var doc = JsonDocument.Parse(json);

        // This returns a string or null, feeding the null check in GetJoke()
        return doc.RootElement.GetProperty("joke").GetString(); 
    }
}