using System;
using System.Net.Http;

namespace CanHazFunny;

/// <summary>
/// Service for retrieving jokes from an external API.  
/// </summary>
public class JokeService : IJokeService
{
    /// <summary>
    /// The HttpClient used to make requests to the joke API.
    /// </summary>
    private HttpClient HttpClient { get; }
 

    /// <summary>
    /// Initializes a new instance of the <see cref="JokeService"/> class with a default <see cref="HttpClient"/>.
    /// </summary>
    public JokeService() : this(new HttpClient()) { }


    /// <summary>
    /// Initializes a new instance of the <see cref="JokeService"/> class with the specified <see cref="HttpClient"/>.
    /// </summary>
    /// <param name="httpClient"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public JokeService(HttpClient httpClient)
    {
        HttpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    }


    /// <summary>
    /// Retrieves a joke from the external API.
    /// </summary>
    /// <returns></returns>
    public string GetJoke()
    {
        return HttpClient.GetStringAsync("https://geek-jokes.sameerkumar.website/api").Result;
    }
}
