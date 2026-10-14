using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace CanHazFunny.Tests;

public class JokeServiceTests
{
    [Fact]
    public void Constructor_WithNullClient_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new JokeService(null!));
    }

    [Fact]
    public void GetJoke_ValidJson_ReturnsJokeText()
    {
        using HttpClient client = CreateClientReturningJson("""{"joke":"Acceptable"}""");

        var service = new JokeService(client);

        Assert.Equal("Acceptable", service.GetJoke());
    }

    [Fact]
    public void GetJoke_WhenCalled_RequestsJsonEndpointUsingGet()
    {
        var handler = new StubHttpMessageHandler("""{"joke":"Acceptable"}""");
        using HttpClient client = new(handler);
        var service = new JokeService(client);

        service.GetJoke();

        Assert.Equal(HttpMethod.Get, handler.RequestMethod);
        Assert.Equal(
            new Uri("https://geek-jokes.sameerkumar.website/api?format=json"),
            handler.RequestUri);
    }

    [Fact]
    public void GetJoke_NullJoke_ThrowsJsonException()
    {
        using HttpClient client = CreateClientReturningJson("""{"joke":null}""");

        var service = new JokeService(client);

        Assert.Throws<JsonException>(service.GetJoke);
    }

    [Fact]
    public void GetJoke_NullResponse_ThrowsJsonException()
    {
        using HttpClient client = CreateClientReturningJson("null");

        var service = new JokeService(client);

        Assert.Throws<JsonException>(service.GetJoke);
    }

    [Fact]
    public void GetJoke_MalformedJson_ThrowsJsonException()
    {
        using HttpClient client = CreateClientReturningJson("""{"joke":null""");

        var service = new JokeService(client);

        Assert.ThrowsAny<JsonException>(service.GetJoke);
    }

    private static HttpClient CreateClientReturningJson(string json)
    {
        return new HttpClient(new StubHttpMessageHandler(json));
    }
}

internal sealed class StubHttpMessageHandler : HttpMessageHandler
{
    private readonly string _json;

    public HttpMethod? RequestMethod { get; private set; }

    public Uri? RequestUri { get; private set; }

    public StubHttpMessageHandler(string json)
    {
        _json = json;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        RequestMethod = request.Method;
        RequestUri = request.RequestUri;

        return Task.FromResult(new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_json)
        });
    }
}
