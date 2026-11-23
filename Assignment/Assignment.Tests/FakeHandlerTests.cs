using System.Net;

namespace Assignment.Tests;

internal sealed class FakeHandler(
    Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>> responder) : HttpMessageHandler
{
    public List<string> Paths { get; } = [];

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        Paths.Add(request.RequestUri!.AbsolutePath);
        return responder(request, cancellationToken);
    }
}

internal sealed class FakeFactory(HttpMessageHandler handler) : IHttpClientFactory
{
    public HttpClient CreateClient(string name) =>
        new(handler, disposeHandler: false) { BaseAddress = new Uri("https://test.local") };
}

internal static class Http
{
    public static HttpResponseMessage Ok(string body) =>
        new(HttpStatusCode.OK) { Content = new StringContent(body) };

    public static HttpResponseMessage Status(HttpStatusCode code) => new(code);

    public static HttpClientWrapper Wrapper(FakeHandler handler) => new(new FakeFactory(handler));
}