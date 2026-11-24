using System.Net;

namespace PeopleEnricher.Tests;

[TestClass]
public class PeopleEndpointClientTests
{
    public required TestContext TestContext { get; set; }

    [TestMethod]
    [DataRow(HttpStatusCode.OK)]
    [DataRow(HttpStatusCode.InternalServerError)]
    public async Task EndpointCalls_ReuseCallerClientAndDisposeResponseContent(HttpStatusCode status)
    {
        TrackingHandler handler = new(status);
        using (HttpClient httpClient = new(handler))
        {
            PeopleEndpointClient client = new(httpClient);
            if (status == HttpStatusCode.OK)
            {
                await client.DownloadPeopleCsvAsync(TestContext.CancellationToken);
                await client.ZipCodeLookupAsync("99201", TestContext.CancellationToken);
            }
            else
            {
                await Assert.ThrowsExactlyAsync<HttpRequestException>(
                    () => client.DownloadPeopleCsvAsync(TestContext.CancellationToken));
                await Assert.ThrowsExactlyAsync<HttpRequestException>(
                    () => client.ZipCodeLookupAsync("99201", TestContext.CancellationToken));
            }

            Assert.HasCount(2, handler.Contents);
            Assert.IsTrue(handler.Contents.All(content => content.IsDisposed));
            Assert.IsFalse(handler.IsDisposed);
            Assert.IsTrue(handler.RequestUris.All(uri => string.IsNullOrEmpty(uri.Query)));
        }
        Assert.IsTrue(handler.IsDisposed);
    }

    private sealed class TrackingHandler(HttpStatusCode status) : HttpMessageHandler
    {
        public List<TrackingContent> Contents { get; } = [];
        public List<Uri> RequestUris { get; } = [];
        public bool IsDisposed { get; private set; }

        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request, CancellationToken cancellationToken)
        {
            RequestUris.Add(request.RequestUri!);
            TrackingContent content = new();
            Contents.Add(content);
            return Task.FromResult(new HttpResponseMessage(status) { Content = content });
        }

        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }

    private sealed class TrackingContent() : StringContent("response")
    {
        public bool IsDisposed { get; private set; }
        protected override void Dispose(bool disposing)
        {
            IsDisposed = true;
            base.Dispose(disposing);
        }
    }
}
