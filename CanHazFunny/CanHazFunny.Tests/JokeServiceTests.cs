
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CanHazFunny;
using Xunit;

namespace CanHazFunny.Tests;

/// <summary>
/// Represents unit tests for the <see cref="JokeService"/> class.
/// </summary>
/// <remarks>
/// This class contains unit tests that verify the behavior of the <see cref="JokeService"/> class, which is responsible for retrieving jokes from the Geek Jokes API. The tests cover various scenarios, including successful joke retrieval, handling of invalid responses, and constructor validation.
/// </remarks>
public class JokeServiceTests
{
    /// <summary>
    /// Tests that the <see cref="JokeService.GetJoke"/> method correctly parses a JSON response and decodes Unicode characters.
    /// </summary> 
    /// <remarks>
    /// This test verifies that the <see cref="JokeService.GetJoke"/> method correctly retrieves a joke from the Geek Jokes API, parses the JSON response, and decodes any Unicode characters present in the joke. It uses a stubbed HTTP message handler to simulate the API response and asserts that the returned joke matches the expected value.
    /// </remarks>
    [Fact]
    public void GetJoke_ParsesJsonAndDecodesUnicode()
    {
        const string responseBody =
            """{"joke":"Everything there\u2019s to know about Google."}""";

        StubHttpMessageHandler handler = new(responseBody);
        using HttpClient client = new(handler);
        JokeService service = new(client);

        string joke = service.GetJoke();

        Assert.Equal(
            "Everything there\u2019s to know about Google.",
            joke);

        Assert.Equal(
            "https://geek-jokes.sameerkumar.website/api?format=json",
            handler.RequestUri?.ToString());

        Assert.Equal(HttpMethod.Get, handler.RequestMethod);
    }

    /// <summary>
    /// Tests that the <see cref="JokeService.GetJoke"/> method throws an <see cref="InvalidOperationException"/> when the response does not contain a valid joke.
    /// </summary>
    /// <param name="responseBody"></param>
    /// <remarks>
    /// This test verifies that the <see cref="JokeService.GetJoke"/> method correctly handles cases where the API response does not contain a valid joke. It uses a theory with inline data to provide different invalid JSON responses and asserts that an <see cref="InvalidOperationException"/> is thrown for each case.
    /// </remarks>
    [Theory]
    [InlineData("""{"other":"value"}""")]
    [InlineData("""{"joke":null}""")]
    [InlineData("""{"joke":123}""")]
    public void GetJoke_ThrowsWhenResponseHasNoStringJoke(
        string responseBody)
    {
        StubHttpMessageHandler handler = new(responseBody);
        using HttpClient client = new(handler);
        JokeService service = new(client);

        Assert.Throws<InvalidOperationException>(
            () => service.GetJoke());
    }

    /// <summary>
    /// Tests that the <see cref="JokeService.GetJoke"/> method throws a <see cref="JsonException"/> when the response is not valid JSON.
    /// </summary>
    /// <remarks>
    /// This test verifies that the <see cref="JokeService.GetJoke"/> method correctly handles cases where the API response is not valid JSON. It uses a stubbed HTTP message handler to simulate an invalid JSON response and asserts that a <see cref="JsonException"/> is thrown when attempting to parse the response.
    /// </remarks>
    [Fact]
    public void GetJoke_ThrowsWhenResponseIsInvalidJson()
    {
        StubHttpMessageHandler handler = new("not JSON");
        using HttpClient client = new(handler);
        JokeService service = new(client);

        Assert.ThrowsAny<JsonException>(
            () => service.GetJoke());
    }

    /// <summary>
    /// Tests that the constructor of the <see cref="JokeService"/> class throws an
    /// <see cref="ArgumentNullException"/> when the <paramref name="httpClient"/> parameter is null.
    /// </summary>
    /// <remarks>
    /// This test verifies that the <see cref="JokeService"/> constructor correctly handles a null <paramref name="httpClient"/> parameter by throwing an <see cref="ArgumentNullException"/>. It asserts that the exception thrown has the expected parameter name.
    /// </remarks>
    [Fact]
    public void Constructor_ThrowsWhenHttpClientIsNull()
    {
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(
                () => new JokeService(null!));

        Assert.Equal("httpClient", exception.ParamName);
    }

    /// <summary>
    /// A stub implementation of <see cref="HttpMessageHandler"/> that returns a predefined response body for testing purposes.
    /// </summary> 
    /// <remarks>
    /// This class is used to simulate HTTP responses from the Geek Jokes API in unit tests. It captures the request URI and method for verification and returns a predefined response body when the <see cref="SendAsync(HttpRequestMessage, CancellationToken)"/> method is called.
    /// </remarks>
    /// <seealso cref="HttpMessageHandler"/>
    /// <seealso cref="HttpClient"/>
    /// <seealso cref="JokeService"/>
    /// <seealso cref="GetJoke_ParsesJsonAndDecodesUnicode"/>
    /// <seealso cref="GetJoke_ThrowsWhenResponseHasNoStringJoke"/>
    /// <seealso cref="GetJoke_ThrowsWhenResponseIsInvalidJson"/>
    /// <seealso cref="Constructor_ThrowsWhenHttpClientIsNull"/>
    /// <seealso cref="JokeServiceTests"/>
    private sealed class StubHttpMessageHandler : HttpMessageHandler
    {
        private readonly string _responseBody;
        public Uri? RequestUri { get; private set; }
        public HttpMethod? RequestMethod { get; private set; }

        public StubHttpMessageHandler(string responseBody)
        {
            _responseBody = responseBody;
        }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            RequestUri = request.RequestUri;
            RequestMethod = request.Method;

            HttpResponseMessage response = new(HttpStatusCode.OK)
            {
                Content = new StringContent(
                    _responseBody,
                    Encoding.UTF8,
                    "application/json")
            };

            return Task.FromResult(response);
        }
    }
}
