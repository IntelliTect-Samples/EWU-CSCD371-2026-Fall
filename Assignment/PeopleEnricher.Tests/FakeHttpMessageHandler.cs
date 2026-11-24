namespace PeopleEnricher.Tests;

internal sealed class FakeHttpMessageHandler(
    Func<HttpRequestMessage, CancellationToken,
        Task<HttpResponseMessage>> responseFactory)
    : HttpMessageHandler
{
    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        return responseFactory(request, cancellationToken);
    }
}