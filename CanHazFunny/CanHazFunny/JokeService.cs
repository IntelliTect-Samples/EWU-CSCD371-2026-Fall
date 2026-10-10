
using System;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CanHazFunny;

/// <summary>
/// Represents a service that provides jokes.
/// </summary>
/// <remarks>
/// This service uses the Geek Jokes API to retrieve jokes in JSON format.
/// </remarks>
/// <seealso href="https://geek-jokes.sameerkumar.website/api?format=json">Geek Jokes API</seealso>
public class JokeService : IJokeService
{
    // The HttpClient instance used to make HTTP requests to the joke service.
    private HttpClient HttpClient { get; }

    // Initializes a new instance of the JokeService class with a default HttpClient.
    public JokeService() : this(new HttpClient())
    {
    }

    /// <summary>
    /// Initializes a new instance of the JokeService class with the specified HttpClient.
    /// </summary>
    /// <param name="httpClient">The HttpClient instance to use for making HTTP requests.</param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="httpClient"/> is null.</exception>
    public JokeService(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient, nameof(httpClient));
        HttpClient = httpClient;
    }

    /// <summary>
    /// Retrieves a joke from the Geek Jokes API as a JSON string, parses the JSON to extract the joke, and returns it as a string.
    /// If the response is invalid or does not contain a joke, an InvalidOperationException is thrown.
    /// </summary>
    /// <returns>A string containing the joke.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the joke service returns no response or an invalid response.</exception>
    /// <remarks>
    public string GetJoke()
    {
        string? response = HttpClient.GetStringAsync(
            "https://geek-jokes.sameerkumar.website/api?format=json").Result;

        if (response == null)
        {
            throw new InvalidOperationException(
                "Joke service returned no response.");
        }

        /// <summary>
        /// Parses the JSON response from the joke service and extracts the joke.
        /// </summary>
        /// <param name="response">The JSON response from the joke service.</param>
        /// <returns>A string containing the joke.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the response does not contain a valid joke.</exception>
        /// <remarks>
        /// The method uses the System.Text.Json library to parse the JSON response and extract the joke. If the response does not contain a valid joke, an InvalidOperationException is thrown.
        /// </remarks>
        JsonDocument document = JsonDocument.Parse(response);

        if (!document.RootElement.TryGetProperty(
            "joke", out JsonElement jokeElement) ||
            jokeElement.ValueKind != JsonValueKind.String)
        {
            throw new InvalidOperationException(
                "Joke service returned no joke.");
        }

        string? joke = jokeElement.GetString();

        if (joke == null)
        {
            throw new InvalidOperationException(
                nameof(JokeService) +
                " returned null after deserialization.");
        }

        return joke;
    }
}
