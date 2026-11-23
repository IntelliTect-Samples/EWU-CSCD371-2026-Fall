using System.Net;

namespace Assignment.Tests;

[TestClass]
public class HttpClientWrapperTests
{
    // ---------- Success ----------

    [TestMethod]
    public async Task DownloadCsvAsyncSuccessReturnsCsvAndCallsPeoplePath()
    {
        var handler = new FakeHandler((_, _) => Task.FromResult(Http.Ok("a,b\n1,2")));

        DownloadResult result = await Http.Wrapper(handler).DownloadPeopleCsvAsync("Ben");

        Assert.AreEqual(DownloadStatus.Success, result.Status);
        Assert.AreEqual("a,b\n1,2", result.Csv);
        Assert.AreEqual("/people/Ben", handler.Paths.Single());
    }

    [TestMethod]
    public async Task DownloadZipAsyncSuccessReturnsBodyAndCallsZipPath()
    {
        var handler = new FakeHandler((_, _) => Task.FromResult(Http.Ok("\"Spokane\",\"WA\"")));

        DownloadResult result = await Http.Wrapper(handler).DownloadZipAsync("99223");

        Assert.AreEqual(DownloadStatus.Success, result.Status);
        Assert.AreEqual("/people/zip/99223", handler.Paths.Single());
    }

    // ---------- Invalid data (both endpoints) ----------

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("\n")]
    public async Task DownloadCsvAsyncBlankBodyReturnsInvalidData(string body)
    {
        var handler = new FakeHandler((_, _) => Task.FromResult(Http.Ok(body)));

        DownloadResult result = await Http.Wrapper(handler).DownloadPeopleCsvAsync("Ben");

        Assert.AreEqual(DownloadStatus.InvalidData, result.Status);
        Assert.IsNull(result.Csv);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    public async Task DownloadZipAsyncBlankBodyReturnsInvalidData(string body)
    {
        var handler = new FakeHandler((_, _) => Task.FromResult(Http.Ok(body)));

        DownloadResult result = await Http.Wrapper(handler).DownloadZipAsync("99223");

        Assert.AreEqual(DownloadStatus.InvalidData, result.Status);
    }

    // ---------- Exception: HTTP status codes and network failures ----------

    [TestMethod]
    [DataRow(HttpStatusCode.NotFound)]
    [DataRow(HttpStatusCode.InternalServerError)]
    [DataRow(HttpStatusCode.ServiceUnavailable)]
    public async Task DownloadCsvAsyncErrorStatusReturnsException(HttpStatusCode code)
    {
        var handler = new FakeHandler((_, _) => Task.FromResult(Http.Status(code)));

        DownloadResult result = await Http.Wrapper(handler).DownloadPeopleCsvAsync("Ben");

        Assert.AreEqual(DownloadStatus.Exception, result.Status);
        Assert.IsInstanceOfType<HttpRequestException>(result.Error);
    }

    [TestMethod]
    [DataRow(HttpStatusCode.NotFound)]
    [DataRow(HttpStatusCode.InternalServerError)]
    public async Task DownloadZipAsyncErrorStatusReturnsException(HttpStatusCode code)
    {
        var handler = new FakeHandler((_, _) => Task.FromResult(Http.Status(code)));

        DownloadResult result = await Http.Wrapper(handler).DownloadZipAsync("99223");

        Assert.AreEqual(DownloadStatus.Exception, result.Status);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DownloadCsvAsyncNetworkFailureReturnsExceptionWithOriginalError()
    {
        var boom = new HttpRequestException("no network");
        var handler = new FakeHandler((_, _) => throw boom);

        DownloadResult result = await Http.Wrapper(handler).DownloadPeopleCsvAsync("Ben");

        Assert.AreEqual(DownloadStatus.Exception, result.Status);
        Assert.AreSame(boom, result.Error);   // original preserved for logging
    }

    [TestMethod]
    public async Task DownloadZipAsyncUnexpectedExceptionReturnsException()
    {
        var handler = new FakeHandler((_, _) => throw new InvalidOperationException("bug"));

        DownloadResult result = await Http.Wrapper(handler).DownloadZipAsync("99223");

        Assert.AreEqual(DownloadStatus.Exception, result.Status);
    }

    // ---------- Timeout: cancellation NOT requested by the caller ----------

    [TestMethod]
    public async Task DownloadCsvAsyncClientTimeoutReturnsTimeout()
    {
        // HttpClient reports its own timeout as TaskCanceledException
        // while the caller's token is still not cancelled.
        var handler = new FakeHandler((_, _) => throw new TaskCanceledException("timeout"));

        DownloadResult result = await Http.Wrapper(handler).DownloadPeopleCsvAsync("Ben", CancellationToken.None);

        Assert.AreEqual(DownloadStatus.Timeout, result.Status);
        Assert.IsNotNull(result.Error);
    }

    [TestMethod]
    public async Task DownloadZipAsyncClientTimeoutReturnsTimeout()
    {
        var handler = new FakeHandler((_, _) => throw new TaskCanceledException("timeout"));

        DownloadResult result = await Http.Wrapper(handler).DownloadZipAsync("99223");

        Assert.AreEqual(DownloadStatus.Timeout, result.Status);
    }

    // ---------- Cancellation: caller cancels the token ----------

    [TestMethod]
    public async Task DownloadCsvAsyncTokenAlreadyCancelledReturnsCancelled()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var handler = new FakeHandler((_, ct) => Task.FromResult(Http.Ok("a,b")));

        DownloadResult result = await Http.Wrapper(handler).DownloadPeopleCsvAsync("Ben", cts.Token);

        Assert.AreEqual(DownloadStatus.Cancelled, result.Status);
        Assert.IsNull(result.Error);   // cancellation is not an error
    }

    [TestMethod]
    public async Task DownloadZipAsyncCancelledWhileInFlightReturnsCancelled()
    {
        using var cts = new CancellationTokenSource();
        var handler = new FakeHandler(async (_, ct) =>
        {
            await Task.Delay(Timeout.Infinite, ct);   // waits until the token fires
            return Http.Ok("never");
        });

        Task<DownloadResult> job = Http.Wrapper(handler).DownloadZipAsync("99223", cts.Token);
        cts.Cancel();

        Assert.AreEqual(DownloadStatus.Cancelled, (await job).Status);
    }

    [TestMethod]
    public async Task DownloadCsvAsyncTokenIsPassedToHandler()
    {
        using var cts = new CancellationTokenSource();
        bool cancelledSeenByHandler = false;

        var handler = new FakeHandler(async (_, ct) =>
        {
            cts.Cancel();                                  // cancel while the request is in flight
            cancelledSeenByHandler = ct.IsCancellationRequested;
            await Task.Yield();
            ct.ThrowIfCancellationRequested();
            return Http.Ok("never");
        });

        DownloadResult result = await Http.Wrapper(handler).DownloadPeopleCsvAsync("Ben", cts.Token);

        Assert.IsTrue(cancelledSeenByHandler);             // handler saw the caller's cancel
        Assert.AreEqual(DownloadStatus.Cancelled, result.Status);
    }
}