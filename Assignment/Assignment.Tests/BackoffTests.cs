using System.Collections.Concurrent;
using System.Net;
using static Assignment.Tests.FakeHandler;

namespace Assignment.Tests;

[TestClass]
public sealed class BackoffTests
{
    private const string Spokane = "\"SPOKANE\",\"WA\"";

    private readonly string _output = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");
    private readonly ConcurrentQueue<TimeSpan> _delays = new();

    public TestContext TestContext { get; set; } = null!;

    [TestCleanup]
    public void Cleanup()
    {
        File.Delete(_output);
        File.Delete(_output + ".tmp");
    }

    // ---------- the backoff formula ----------

    [TestMethod]
    [DataRow(1, 200)]
    [DataRow(2, 400)]
    [DataRow(3, 800)]
    [DataRow(4, 1600)]
    [DataRow(5, 3200)]
    [DataRow(6, 5000)]    // 6400 is capped at MaxDelay
    [DataRow(20, 5000)]
    public void BackoffDoublesPerFailureUntilCapped(int failedAttempts, int fullMilliseconds)
    {
        Assert.AreEqual(fullMilliseconds / 2.0, PeopleEnricherService.Backoff(failedAttempts, jitter: 0).TotalMilliseconds, 0.001);
        Assert.AreEqual(fullMilliseconds * 0.75, PeopleEnricherService.Backoff(failedAttempts, jitter: 0.5).TotalMilliseconds, 0.001);
        Assert.AreEqual(fullMilliseconds, PeopleEnricherService.Backoff(failedAttempts, jitter: 0.999_999).TotalMilliseconds, 0.01);
    }

    [TestMethod]
    public void JitterSpreadsDelaysBetweenHalfAndFull()
    {
        TimeSpan[] delays = [.. Enumerable.Range(0, 200).Select(_ => PeopleEnricherService.Backoff(2, Random.Shared.NextDouble()))];

        Assert.IsTrue(delays.All(d => d.TotalMilliseconds is >= 200 and < 400));
        Assert.IsGreaterThan(1, delays.Distinct().Count());   // not every retry waits the same amount
    }

    // ---------- backoff inside the retry loop ----------

    [TestMethod]
    public async Task WaitsGrowBetweenFailedAttempts()
    {
        using FakeHandler handler = new(call => call switch
        {
            { Endpoint: Endpoint.People } => Ok(PeopleCsv("99201")),
            { Number: <= 2 } => Status(HttpStatusCode.ServiceUnavailable),
            _ => Ok(Spokane),
        });

        RunSummary summary = await RunAsync(handler);

        Assert.AreEqual(1, summary.Written);
        TimeSpan[] delays = [.. _delays];
        Assert.HasCount(2, delays);   // one wait after each of the 2 failures, none after the success
        Assert.IsTrue(delays[0].TotalMilliseconds is >= 100 and < 200);
        Assert.IsTrue(delays[1].TotalMilliseconds is >= 200 and < 400);
    }

    [TestMethod]
    public async Task NoWaitAfterTheLastAttempt()
    {
        using FakeHandler handler = new(call =>
            call.Endpoint == Endpoint.People ? Ok(PeopleCsv("99201")) : Status(HttpStatusCode.ServiceUnavailable));

        await RunAsync(handler, maxAttempts: 3);

        Assert.AreEqual(3, handler.ZipCalls);
        Assert.HasCount(2, _delays);   // between attempts 1-2 and 2-3 only
    }

    [TestMethod]
    public async Task NoWaitWhenNothingFails()
    {
        using FakeHandler handler = new(call => call.Endpoint == Endpoint.People ? Ok(PeopleCsv("99201", "99202")) : Ok(Spokane));

        await RunAsync(handler);

        Assert.IsEmpty(_delays);
    }

    [TestMethod]
    public async Task NotFoundIsNotRetriedSoNoWait()
    {
        using FakeHandler handler = new(call =>
            call.Endpoint == Endpoint.People ? Ok(PeopleCsv("99201")) : Status(HttpStatusCode.NotFound));

        await RunAsync(handler);

        Assert.IsEmpty(_delays);
    }

    [TestMethod]
    public async Task CancellingDuringAWaitStopsTheRun()
    {
        using CancellationTokenSource cts = new();
        using FakeHandler handler = new(call =>
            call.Endpoint == Endpoint.People ? Ok(PeopleCsv("99201")) : Status(HttpStatusCode.ServiceUnavailable));
        using HttpClient http = new(handler, disposeHandler: false) { BaseAddress = new Uri("http://test") };
        PeopleEnricherService service = new(http)
        {
            Delay = async (_, token) =>
            {
                await cts.CancelAsync();
                await Task.Delay(Timeout.Infinite, token);   // a long wait that only cancellation ends
            },
        };

        await Assert.ThrowsAsync<OperationCanceledException>(() => service.RunAsync("tester", _output, cts.Token));

        Assert.AreEqual(1, handler.ZipCalls);   // no further attempts after the cancelled wait
        Assert.IsFalse(File.Exists(_output));
    }

    [TestMethod]
    public async Task DefaultDelayReallyWaits()
    {
        using FakeHandler handler = new(call => call switch
        {
            { Endpoint: Endpoint.People } => Ok(PeopleCsv("99201")),
            { Number: 1 } => Status(HttpStatusCode.ServiceUnavailable),
            _ => Ok(Spokane),
        });
        using HttpClient http = new(handler, disposeHandler: false) { BaseAddress = new Uri("http://test") };
        PeopleEnricherService service = new(http);   // real Task.Delay
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await service.RunAsync("tester", _output, TestContext.CancellationToken);

        Assert.IsGreaterThanOrEqualTo(90, stopwatch.ElapsedMilliseconds);   // first backoff is at least 100ms (timer slack allowed)
    }

    private async Task<RunSummary> RunAsync(FakeHandler handler, int maxAttempts = 5)
    {
        using HttpClient http = new(handler, disposeHandler: false) { BaseAddress = new Uri("http://test") };
        PeopleEnricherService service = new(http, maxAttempts)
        {
            Delay = (delay, _) =>
            {
                _delays.Enqueue(delay);
                return Task.CompletedTask;
            },
        };
        return await service.RunAsync("tester", _output, TestContext.CancellationToken);
    }
}
