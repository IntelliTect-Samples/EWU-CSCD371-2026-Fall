using System.Net;
using static Assignment.Tests.FakeHandler;

namespace Assignment.Tests;

[TestClass]
public sealed class ResilienceTests
{
    private const string Spokane = "\"SPOKANE\",\"WA\"";
    private const string LookupFailed = "ZIP lookup failed or returned invalid data";

    private readonly string _output = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");

    public TestContext TestContext { get; set; } = null!;

    [TestCleanup]
    public void Cleanup()
    {
        File.Delete(_output);
        File.Delete(_output + ".tmp");
    }

    // ---------- people endpoint ----------

    [TestMethod]
    public async Task HappyPathWritesEnrichedCsv()
    {
        using FakeHandler handler = new(call => call.Endpoint == Endpoint.People ? Ok(PeopleCsv("99201")) : Ok(Spokane));

        RunSummary summary = await RunAsync(handler);

        Assert.AreEqual(1, summary.Written);
        string[] lines = await File.ReadAllLinesAsync(_output, TestContext.CancellationToken);
        CollectionAssert.AreEqual(
            new[] { PeopleEnricherService.OutputHeader, "First0,Last0,p0@example.com,99201,SPOKANE,WA" }, lines);
    }

    [TestMethod]
    [DataRow("Exception")]
    [DataRow("InvalidData")]
    [DataRow("Timeout")]
    public async Task PeopleDownloadRetriesEachFailureMode(string mode)
    {
        using FakeHandler handler = new(call => call switch
        {
            { Endpoint: Endpoint.People, Number: 1 } => Failure(mode),
            { Endpoint: Endpoint.People } => Ok(PeopleCsv("99201")),
            _ => Ok(Spokane),
        });

        RunSummary summary = await RunAsync(handler);

        Assert.AreEqual(1, summary.Written);
        Assert.AreEqual(3, handler.PeopleCalls);   // 1 failure, then 2 good downloads that agree
    }

    [TestMethod]
    public async Task PeopleDownloadIgnoresCsvWithSwappedZips()
    {
        using FakeHandler handler = new(call => call switch
        {
            { Endpoint: Endpoint.People, Number: 1 } => Ok(PeopleCsv("12345")),   // looks valid, but wrong
            { Endpoint: Endpoint.People } => Ok(PeopleCsv("99201")),
            _ => Ok(Spokane),
        });

        await RunAsync(handler);

        string[] lines = await File.ReadAllLinesAsync(_output, TestContext.CancellationToken);
        StringAssert.Contains(lines[1], ",99201,");
    }

    [TestMethod]
    public async Task PeopleDownloadFailingForGoodThrowsAndWritesNothing()
    {
        using FakeHandler handler = new(_ => Status(HttpStatusCode.InternalServerError));

        InvalidOperationException ex = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() => RunAsync(handler));

        Assert.IsInstanceOfType<HttpRequestException>(ex.InnerException);
        Assert.IsFalse(File.Exists(_output));
        Assert.IsFalse(File.Exists(_output + ".tmp"));
    }

    // ---------- zip endpoint ----------

    [TestMethod]
    [DataRow("Exception")]
    [DataRow("InvalidData")]
    [DataRow("Timeout")]
    public async Task ZipLookupRetriesEachFailureMode(string mode)
    {
        using FakeHandler handler = new(call => call switch
        {
            { Endpoint: Endpoint.People } => Ok(PeopleCsv("99201")),
            { Number: 1 } => Failure(mode),
            _ => Ok(Spokane),
        });

        RunSummary summary = await RunAsync(handler);

        Assert.AreEqual(1, summary.Written);
        Assert.AreEqual(2, handler.ZipCalls);
    }

    [TestMethod]
    [DataRow("Exception")]
    [DataRow("InvalidData")]
    [DataRow("Timeout")]
    public async Task ZipLookupFailingForGoodRemovesOnlyThatPerson(string mode)
    {
        using FakeHandler handler = new(call => call switch
        {
            { Endpoint: Endpoint.People } => Ok(PeopleCsv("99201", "11111")),
            { Path: "/people/zip/11111" } => Failure(mode),
            _ => Ok(Spokane),
        });

        RunSummary summary = await RunAsync(handler);

        Assert.AreEqual(1, summary.Written);
        Assert.AreEqual(1, summary.RejectionCounts[LookupFailed]);
    }

    [TestMethod]
    public async Task ZipNotFoundIsNotRetried()
    {
        using FakeHandler handler = new(call =>
            call.Endpoint == Endpoint.People ? Ok(PeopleCsv("99201")) : Status(HttpStatusCode.NotFound));

        RunSummary summary = await RunAsync(handler);

        Assert.AreEqual(0, summary.Written);
        Assert.AreEqual(1, handler.ZipCalls);
    }

    // ---------- unexpected exceptions and cancellation ----------

    [TestMethod]
    public async Task UnexpectedExceptionPropagatesWithoutRetry()
    {
        using FakeHandler handler = new(call =>
            call.Endpoint == Endpoint.People ? Ok(PeopleCsv("99201")) : throw new FormatException("bug"));

        await Assert.ThrowsExactlyAsync<FormatException>(() => RunAsync(handler));

        Assert.AreEqual(1, handler.ZipCalls);
        Assert.IsFalse(File.Exists(_output));
    }

    [TestMethod]
    public async Task AlreadyCancelledTokenMakesNoRequests()
    {
        using CancellationTokenSource cts = new();
        await cts.CancelAsync();
        using FakeHandler handler = new(_ => Ok(PeopleCsv("99201")));

        await Assert.ThrowsAsync<OperationCanceledException>(() => RunAsync(handler, cts.Token));

        Assert.AreEqual(0, handler.PeopleCalls);
    }

    [TestMethod]
    public async Task CancellingDuringLookupStopsWithoutRetryOrOutput()
    {
        using CancellationTokenSource cts = new();
        using FakeHandler handler = new(call =>
        {
            if (call.Endpoint == Endpoint.People)
            {
                return Ok(PeopleCsv("99201"));
            }

            cts.Cancel();
            return null;   // stall until the cancellation reaches the request
        });

        await Assert.ThrowsAsync<OperationCanceledException>(() => RunAsync(handler, cts.Token));

        Assert.AreEqual(1, handler.ZipCalls);
        Assert.IsFalse(File.Exists(_output));
        Assert.IsFalse(File.Exists(_output + ".tmp"));
    }

    // ---------- helpers ----------

    private static HttpResponseMessage? Failure(string mode) => mode switch
    {
        "Exception" => Status((HttpStatusCode)418),
        "InvalidData" => Ok("BAD_DATA"),
        "Timeout" => null,
        _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null),
    };

    private async Task<RunSummary> RunAsync(FakeHandler handler, CancellationToken? cancellationToken = null)
    {
        using HttpClient http = new(handler, disposeHandler: false)
        {
            BaseAddress = new Uri("http://test"),
            Timeout = TimeSpan.FromMilliseconds(300),
        };
        PeopleEnricherService service = new(http, maxAttempts: 3);
        return await service.RunAsync("tester", _output, cancellationToken ?? TestContext.CancellationToken);
    }
}
