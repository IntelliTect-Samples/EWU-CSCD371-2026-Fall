using System.Net;

namespace PeopleEnricher.Tests;

[TestClass]
public class ResilienceTests
{
    private const string Header = "FirstName,LastName,EmailAddress,ZipCode";
    private const string Row = "Ada,Lovelace,ada@example.com,99201";
    private const string Csv = Header + "\n" + Row;
    private const string Location = "\"SPOKANE\",\"WA\"";
    public required TestContext TestContext { get; set; }

    [TestMethod]
    [DataRow(true, "http")]
    [DataRow(true, "timeout")]
    [DataRow(true, "cancelled")]
    [DataRow(false, "http")]
    [DataRow(false, "timeout")]
    [DataRow(false, "cancelled")]
    public async Task EnrichAsync_TransientEndpointFailure_RetriesAndSucceeds(bool download, string failure)
    {
        int attempts = 0;
        Task<string> Request()
        {
            attempts++;
            return attempts == 1
                ? Task.FromException<string>(Failure(failure, download))
                : Task.FromResult(download ? Csv : Location);
        }

        StubPeopleEndpointClient client = new(
            downloadPeopleCsv: () => download ? Request() : Task.FromResult(Csv),
            zipCodeLookup: _ => download ? Task.FromResult(Location) : Request());
        PeopleEnricher enricher = new(client, maxAttempts: 2);

        EnrichmentResult result = await enricher.EnrichAsync(TestContext.CancellationToken);

        Assert.AreEqual(2, attempts);
        Assert.IsEmpty(result.RejectedRows);
        StringAssert.Contains(result.EnrichedCsv, Row + ",Spokane,WA");
    }

    [TestMethod]
    [DataRow(true, "http")]
    [DataRow(true, "timeout")]
    [DataRow(true, "cancelled")]
    [DataRow(false, "http")]
    [DataRow(false, "timeout")]
    [DataRow(false, "cancelled")]
    public async Task EnrichAsync_ExhaustedRetries_FailsDownloadOrReportsPerson(bool download, string failure)
    {
        int attempts = 0;
        Task<string> Request()
        {
            attempts++;
            return Task.FromException<string>(Failure(failure, download));
        }

        StubPeopleEndpointClient client = new(
            downloadPeopleCsv: () => download ? Request() : Task.FromResult(Csv),
            zipCodeLookup: _ => download ? Task.FromResult(Location) : Request());
        PeopleEnricher enricher = new(client, maxAttempts: 2);

        if (download)
        {
            EnrichmentException exception = await Assert.ThrowsExactlyAsync<EnrichmentException>(
                () => enricher.EnrichAsync(TestContext.CancellationToken));
            Assert.AreEqual(failure == "http" ? EnrichmentErrorType.CsvDownloadFailed
                : EnrichmentErrorType.CsvDownloadTimeout, exception.ErrorType);
        }
        else
        {
            EnrichmentResult result = await enricher.EnrichAsync(TestContext.CancellationToken);
            RejectedRow rejected = result.RejectedRows.Single();
            Assert.AreEqual(2, rejected.LineNumber);
            Assert.AreEqual(Row, rejected.RawData);
            Assert.IsFalse(string.IsNullOrWhiteSpace(rejected.Reason));
            Assert.HasCount(1, result.EnrichedCsv.Split('\n', StringSplitOptions.RemoveEmptyEntries));
        }
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    [DataRow("BAD_DATA")]
    [DataRow("\"\",\"WA\"")]
    [DataRow("\"SPOKANE\",\"\"")]
    [DataRow("\"SPOKANE\",\"Washington\"")]
    [DataRow("\"\"SPOKANE\"\",\"WA\"")]
    public async Task EnrichAsync_BadLocation_RejectsWithoutRetrying(string location)
    {
        int calls = 0;
        StubPeopleEndpointClient client = new(
            downloadPeopleCsv: () => Task.FromResult(Csv),
            zipCodeLookup: _ => { calls++; return Task.FromResult(location); });

        EnrichmentResult result = await new PeopleEnricher(client).EnrichAsync(TestContext.CancellationToken);

        Assert.HasCount(1, result.RejectedRows);
        Assert.AreEqual(Row, result.RejectedRows[0].RawData);
        Assert.AreEqual(1, calls);
        Assert.IsFalse(result.EnrichedCsv.Contains(Row, StringComparison.Ordinal));
    }

    [TestMethod]
    [DataRow("Ada,Lovelace,ada@example.com")]
    [DataRow("Ada,Lovelace,ada@example.com,99201,extra")]
    [DataRow(" ,Lovelace,ada@example.com,99201")]
    [DataRow("Ada,Lovelace,,99201")]
    [DataRow("Ada,Lovelace,ada@example.com,abcde")]
    [DataRow("Ada,Lovelace,ada@example.com,１２３４５")]
    public async Task EnrichAsync_BadPerson_ReportsRowAndKeepsValidPerson(string badRow)
    {
        int lookups = 0;
        StubPeopleEndpointClient client = new(
            downloadPeopleCsv: () => Task.FromResult(Header + "\n" + badRow + "\n" + Row),
            zipCodeLookup: _ => { lookups++; return Task.FromResult(Location); });

        EnrichmentResult result = await new PeopleEnricher(client).EnrichAsync(TestContext.CancellationToken);

        Assert.AreEqual(1, lookups);
        Assert.HasCount(1, result.RejectedRows);
        Assert.AreEqual(2, result.RejectedRows[0].LineNumber);
        Assert.AreEqual(badRow, result.RejectedRows[0].RawData);
        StringAssert.Contains(result.EnrichedCsv, Row + ",Spokane,WA");
    }

    [TestMethod]
    public async Task EnrichAsync_MissingZip_IsRejectedWithoutRetry()
    {
        int calls = 0;
        StubPeopleEndpointClient client = new(
            downloadPeopleCsv: () => Task.FromResult(Csv),
            zipCodeLookup: _ =>
            {
                calls++;
                throw new HttpRequestException("Missing ZIP", null, HttpStatusCode.NotFound);
            });

        EnrichmentResult result = await new PeopleEnricher(client).EnrichAsync(TestContext.CancellationToken);

        Assert.AreEqual(1, calls);
        Assert.HasCount(1, result.RejectedRows);
    }

    [TestMethod]
    public async Task EnrichAsync_LookupsOverlapButRespectConcurrencyLimit()
    {
        TaskCompletionSource twoStarted = new(TaskCreationOptions.RunContinuationsAsynchronously);
        TaskCompletionSource release = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int active = 0;
        int peak = 0;
        int calls = 0;
        StubPeopleEndpointClient client = new(
            downloadPeopleCsv: () => Task.FromResult(Header + "\n" +
                string.Join('\n', Enumerable.Range(1, 4).Select(i => $"Ada,Lovelace,ada@example.com,{99200 + i}"))),
            zipCodeLookupWithCancellation: async (_, token) =>
            {
                int current = Interlocked.Increment(ref active);
                Interlocked.Increment(ref calls);
                if (current > peak) peak = current;
                if (current == 2) twoStarted.TrySetResult();
                try { await release.Task.WaitAsync(token); return Location; }
                finally { Interlocked.Decrement(ref active); }
            });
        PeopleEnricher enricher = new(client, maxConcurrency: 2);
        Task<EnrichmentResult> pending = enricher.EnrichAsync(TestContext.CancellationToken);
        try
        {
            await twoStarted.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.CancellationToken);
            Assert.AreEqual(2, active);
            Assert.AreEqual(2, calls);
        }
        finally { release.TrySetResult(); }
        EnrichmentResult result = await pending;
        Assert.AreEqual(2, peak);
        Assert.AreEqual(4, calls);
        Assert.IsEmpty(result.RejectedRows);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task EnrichAsync_CallerCancellation_StopsRequestsAndQueuedLookups(bool download)
    {
        TaskCompletionSource started = new(TaskCreationOptions.RunContinuationsAsynchronously);
        int active = 0;
        async Task<string> Block(CancellationToken token)
        {
            Interlocked.Increment(ref active);
            started.TrySetResult();
            try { await Task.Delay(Timeout.InfiniteTimeSpan, token); return Location; }
            finally { Interlocked.Decrement(ref active); }
        }

        using CancellationTokenSource cancellation =
            CancellationTokenSource.CreateLinkedTokenSource(TestContext.CancellationToken);
        StubPeopleEndpointClient client = new(
            downloadPeopleCsvWithCancellation: token => download ? Block(token)
                : Task.FromResult(Csv + "\nAda,Lovelace,ada@example.com,99202"),
            zipCodeLookupWithCancellation: (_, token) => Block(token));
        PeopleEnricher enricher = new(client, maxConcurrency: 1);
        Task<EnrichmentResult> pending = enricher.EnrichAsync(cancellation.Token);
        await started.Task.WaitAsync(TimeSpan.FromSeconds(5), TestContext.CancellationToken);
        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(() => pending);
        Assert.AreEqual(0, active);
    }

    [TestMethod]
    public async Task EnrichAsync_RequestTimeout_CancelsAndFinishesEachAttempt()
    {
        int finished = 0;
        StubPeopleEndpointClient client = new(
            downloadPeopleCsv: () => Task.FromResult(Csv),
            zipCodeLookupWithCancellation: async (_, token) =>
            {
                try { await Task.Delay(Timeout.InfiniteTimeSpan, token); return Location; }
                finally { Interlocked.Increment(ref finished); }
            });
        PeopleEnricher enricher = new(client, TimeSpan.FromMilliseconds(30), maxAttempts: 2);

        EnrichmentResult result = await enricher.EnrichAsync(TestContext.CancellationToken);

        Assert.AreEqual(2, finished);
        Assert.HasCount(1, result.RejectedRows);
    }

    [TestMethod]
    [DataRow(true)]
    [DataRow(false)]
    public async Task EnrichAsync_UnexpectedException_PropagatesSameInstance(bool download)
    {
        InvalidOperationException failure = new("Unexpected failure");
        StubPeopleEndpointClient client = new(
            downloadPeopleCsv: () => download ? throw failure : Task.FromResult(Csv),
            zipCodeLookup: _ => throw failure);

        InvalidOperationException actual = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => new PeopleEnricher(client).EnrichAsync(TestContext.CancellationToken));

        Assert.AreSame(failure, actual);
    }

    [TestMethod]
    public async Task EnrichAsync_CacheIsSharedWithinRunButRefreshedForNextRun()
    {
        int calls = 0;
        StubPeopleEndpointClient client = new(
            downloadPeopleCsv: () => Task.FromResult(Csv + "\n" + Row),
            zipCodeLookup: _ => ++calls == 1 ? throw new HttpRequestException() : Task.FromResult(Location));
        PeopleEnricher enricher = new(client, maxAttempts: 1);

        EnrichmentResult first = await enricher.EnrichAsync(TestContext.CancellationToken);
        EnrichmentResult second = await enricher.EnrichAsync(TestContext.CancellationToken);

        Assert.HasCount(2, first.RejectedRows);
        Assert.IsEmpty(second.RejectedRows);
        Assert.AreEqual(2, calls);
    }

    private static Exception Failure(string failure, bool download) => failure switch
    {
        "http" => new HttpRequestException("Service failure", null, download ? HttpStatusCode.InternalServerError : (HttpStatusCode)418),
        "timeout" => new TimeoutException(),
        "cancelled" => new TaskCanceledException(),
        _ => throw new ArgumentException("Unknown failure", nameof(failure))
    };
}
