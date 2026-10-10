
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

public class JokeServiceTests
{
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

    [Fact]
    public void GetJoke_ThrowsWhenResponseIsInvalidJson()
    {
        StubHttpMessageHandler handler = new("not JSON");
        using HttpClient client = new(handler);
        JokeService service = new(client);

        Assert.ThrowsAny<JsonException>(
            () => service.GetJoke());
    }

    [Fact]
    public void Constructor_ThrowsWhenHttpClientIsNull()
    {
        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(
                () => new JokeService(null!));

        Assert.Equal("httpClient", exception.ParamName);
    }

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
