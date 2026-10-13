using System;
using System.Net.Http;
using System.Text.Json;


namespace CanHazFunny;

public class JokeService : IJokeService
{
    private readonly HttpClient JokeHttpClient;

    private const string JOKEURL = "https://geek-jokes.sameerkumar.website/api";

    public JokeService() : this(new HttpClient()){

    }
    public JokeService(HttpClient httpClient){
        ArgumentNullException.ThrowIfNull(httpClient); // added nullcheck
        JokeHttpClient = httpClient;
    }

    public string GetJoke()
    {
        string json = JokeHttpClient.GetStringAsync(JOKEURL).GetAwaiter().GetResult();
        //string joke = httpClient.GetStringAsync("https://geek-jokes.sameerkumar.website/api").Result;
        string? joke = JsonSerializer.Deserialize<string>(json);
        if (joke is null)
            {
                throw new InvalidOperationException("The response did not contain a joke.");
            }
        return joke; 
    }
}
