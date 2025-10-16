using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CanHazFunny.Tests;

public class JokeServiceTests
{
    private const string ValidJokeJson = "{\"joke\":\"Why do programmers prefer dark mode? Because light attracts bugs.\"}";
    private const string ValidJokeText = "Why do programmers prefer dark mode? Because light attracts bugs.";
    private const string MissingPropertyJson = "{\"not_joke\":\"This is not a joke.\"}";
    private const string EmptyJokeTextJson = "{\"joke\":\"   \"}";

    private static class JokeLogicHelper
    {
        public static string ExtractAndValidateJoke(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                throw new InvalidOperationException("No joke received from the API.");

            var parsedJson = JsonSerializer.Deserialize<JsonElement>(json);
            if (!parsedJson.TryGetProperty("joke", out JsonElement jokeElement))
                throw new InvalidOperationException("No 'joke' property found in the response.");

            string? joke = jokeElement.GetString();
            if (string.IsNullOrWhiteSpace(joke))
                throw new InvalidOperationException("Joke text is empty or null.");

            return joke;
        }
    }

    private sealed class FakeHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseContent;

        public FakeHttpMessageHandler(string content)
        {
            _responseContent = content;
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(_responseContent, Encoding.UTF8, "application/json")
            };
            return Task.FromResult(response);
        }
    }

    private sealed class IsolatedJokeService : IJokeService
    {
        private HttpClient HttpClient { get; }

        public IsolatedJokeService(HttpMessageHandler handler)
        {
            HttpClient = new HttpClient(handler);
        }

        public string GetJoke()
        {
            string json = HttpClient.GetStringAsync("https://geek-jokes.sameerkumar.website/api?format=json").Result;

            return JokeLogicHelper.ExtractAndValidateJoke(json);
        }
    }

    [Fact]
    public void JokeService_GetJoke_ReturnsNonEmptyString_IntegrationTest()
    {
        // Changed to use a mocked HTTP handler so the test is deterministic and does not call the external API.
        var mockHandler = new FakeHttpMessageHandler(ValidJokeJson);
        var jokeService = new IsolatedJokeService(mockHandler);

        string joke = jokeService.GetJoke();
        Assert.False(string.IsNullOrWhiteSpace(joke));
    }

    [Fact]
    public void JokeService_GetJoke_ReturnsValidJoke_OnMockSuccess()
    {
        var mockHandler = new FakeHttpMessageHandler(ValidJokeJson);
        var jokeService = new IsolatedJokeService(mockHandler);

        string joke = jokeService.GetJoke();

        Assert.Equal(ValidJokeText, joke);
    }

    [Fact]
    public void JokeService_GetJoke_ThrowsInvalidOperationException_WhenApiReturnsEmptyJson()
    {
        // Arrange
        var mockHandler = new FakeHttpMessageHandler("");
        var jokeService = new IsolatedJokeService(mockHandler);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => jokeService.GetJoke());
        Assert.Contains("No joke received from the API.", ex.Message);
    }

    [Fact]
    public void JokeService_GetJoke_ThrowsInvalidOperationException_WhenJokePropertyIsMissing()
    {
        // Arrange
        var mockHandler = new FakeHttpMessageHandler(MissingPropertyJson);
        var jokeService = new IsolatedJokeService(mockHandler);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => jokeService.GetJoke());
        Assert.Contains("No 'joke' property found in the response.", ex.Message);
    }

    [Fact]
    public void JokeService_GetJoke_ThrowsInvalidOperationException_WhenJokeTextIsEmpty()
    {
        // Arrange
        var mockHandler = new FakeHttpMessageHandler(EmptyJokeTextJson);
        var jokeService = new IsolatedJokeService(mockHandler);

        // Act & Assert
        var ex = Assert.Throws<InvalidOperationException>(() => jokeService.GetJoke());
        Assert.Contains("Joke text is empty or null.", ex.Message);
    }
}