using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CanHazFunny.Tests;

/// <summary>
/// Unit tests for the JokeService class. These tests verify that the JokeService correctly retrieves jokes from the external API and handles various response scenarios. The tests use a stubbed HttpMessageHandler to simulate API responses, allowing for deterministic testing without relying on external services.
/// </summary>
public class JokeServiceTests
{

    /// <summary>
    /// Tests that the default constructor of the JokeService class creates a non-null instance of the service. This test ensures that the JokeService can be instantiated without any parameters, and that it initializes its internal HttpClient correctly.
    /// </summary>
    [Fact]
    public void DefaultConstructorCreatesService()
    {
        JokeService service = new();

        Assert.NotNull(service);
    }


    /// <summary>
    /// Tests that the constructor of the JokeService class throws an ArgumentNullException when a null HttpClient is passed as a parameter. This test ensures that the JokeService correctly validates its dependencies and enforces non-null requirements for its constructor parameters.
    /// </summary>
    [Fact]
    public void ConstructorThrowsWhenHttpClientIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
            () => new JokeService(null!));

        Assert.Equal("httpClient", exception.ParamName);
    }


    /// <summary>
    /// Tests that the GetJoke method returns the expected joke when the external API responds with a successful HTTP status code (200 OK). The test uses a stubbed HttpMessageHandler to simulate the API response, ensuring that the JokeService correctly processes the response and returns the joke string.
    /// </summary>
    [Fact]
    public void GetJokeReturnsResponseBody()
    {
        const string joke = "A deterministic joke.";
        using HttpClient httpClient = new(new StubResponseHandler(request =>
        {
            Assert.Equal("https://geek-jokes.sameerkumar.website/api", request.RequestUri?.ToString());
            return new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent(joke)
            };
        }));
        JokeService service = new(httpClient);

        Assert.Equal(joke, service.GetJoke());
    }


    /// <summary>
    /// Tests that the GetJoke method throws an AggregateException when the external API responds with an unsuccessful HTTP status code (e.g., 503 Service Unavailable). The test uses a stubbed HttpMessageHandler to simulate the API response, ensuring that the JokeService correctly handles error scenarios and propagates exceptions as expected.
    /// </summary>
    [Fact]
    public void GetJokeThrowsWhenResponseIsUnsuccessful()
    {
        using HttpClient httpClient = new(new StubResponseHandler(_ =>
            new HttpResponseMessage(HttpStatusCode.ServiceUnavailable)));
        JokeService service = new(httpClient);

        AggregateException exception = Assert.Throws<AggregateException>(() => service.GetJoke());

        Assert.IsType<HttpRequestException>(exception.InnerException);
    }


    /// <summary>
    /// A stubbed HttpMessageHandler that allows for custom responses to be returned based on the incoming HttpRequestMessage. This class is used in unit tests to simulate various API responses without making actual HTTP requests, enabling deterministic testing of the JokeService class.
    /// </summary>
    /// <param name="createResponse"></param>
    private sealed class StubResponseHandler(
        Func<HttpRequestMessage, HttpResponseMessage> createResponse) : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(createResponse(request));
        }
    }
}

