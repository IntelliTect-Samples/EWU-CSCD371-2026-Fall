using Microsoft.Kiota.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment9.Tests;

[TestClass]
public sealed class PeopleEnricherTests(TestContext testContext)
{
    private static readonly string[] expectedOutput =
    [
        EnrichedPerson.CsvHeader,
        "Ada,Lovelace,ada@example.com,99201,SPOKANE,WA",
        "Grace,Hopper,grace@example.com,98101,SEATTLE,WA",
        "Alan,Turing,alan@example.com,97201,PORTLAND,OR",
    ];

    // ---- Happy path -------------------------------------------------------------------------

    [TestMethod]
    public async Task RunAsync_EverythingSucceeds_WritesEnrichedCsv()
    {
        using var api = new FakeMayhemApi();
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();
        string output = temp.PathOf("out.csv");

        EnrichmentReport report = await CreateEnricher(connection.Client).RunAsync(SampleData.User, output, testContext.CancellationToken);

        CollectionAssert.AreEqual(expectedOutput, await File.ReadAllLinesAsync(output, testContext.CancellationToken));
        Assert.AreEqual(3, report.RowsDownloaded);
        Assert.HasCount(3, report.Enriched);
        Assert.IsEmpty(report.Rejections);
        CollectionAssert.AreEqual(new[] { output }, temp.Files());
        Assert.AreEqual(4, api.BodyStreamCount, "One body stream per successful response.");
        Assert.IsTrue(api.AllBodyStreamsDisposed);
    }

    [TestMethod]
    public async Task RunAsync_WritesTheLookedUpCityAndStateExactly()
    {
        using var api = new FakeMayhemApi { Zip = (_, _, _) => Responses.Ok("\"ST. LOUIS\",\"MO\"") };
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();
        string output = temp.PathOf("out.csv");

        await CreateEnricher(connection.Client).RunAsync(SampleData.User, output, testContext.CancellationToken);

        string[] lines = await File.ReadAllLinesAsync(output, testContext.CancellationToken);
        Assert.AreEqual("Ada,Lovelace,ada@example.com,99201,ST. LOUIS,MO", lines[1]);
    }

    // ---- People endpoint failure modes ----------------------------------------------------

    [TestMethod]
    public async Task RunAsync_PeopleEndpointFailsThenRecovers_Succeeds()
    {
        using var api = new FakeMayhemApi
        {
            People = (attempt, token) => attempt switch
            {
                1 => Responses.ServerError(),
                2 => Responses.Hang(token),
                3 => Responses.InvalidData(),
                _ => Responses.Ok(SampleData.PeopleCsv),
            },
        };
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();
        string output = temp.PathOf("out.csv");

        await CreateEnricher(connection.Client, maxAttempts: 4).RunAsync(SampleData.User, output, testContext.CancellationToken);

        Assert.AreEqual(4, api.AttemptsFor(SampleData.PeoplePath));
        CollectionAssert.AreEqual(expectedOutput, await File.ReadAllLinesAsync(output, testContext.CancellationToken));
        Assert.IsTrue(api.AllBodyStreamsDisposed);
    }

    [TestMethod]
    public async Task RunAsync_PeopleEndpointAlwaysErrors_ThrowsAndWritesNothing()
    {
        using var api = new FakeMayhemApi { People = (_, _) => Responses.ServerError() };
        await AssertPeopleDownloadFailsAsync<ApiException>(api);
    }

    [TestMethod]
    public async Task RunAsync_PeopleEndpointAlwaysTimesOut_ThrowsAndWritesNothing()
    {
        using var api = new FakeMayhemApi { People = (_, token) => Responses.Hang(token) };
        await AssertPeopleDownloadFailsAsync<TimeoutException>(api);
    }

    [TestMethod]
    public async Task RunAsync_PeopleEndpointAlwaysInvalid_ThrowsAndWritesNothing()
    {
        using var api = new FakeMayhemApi { People = (_, _) => Responses.InvalidData() };
        await AssertPeopleDownloadFailsAsync<InvalidResponseException>(api);
    }

    // ---- Bad people data ------------------------------------------------------------------

    [TestMethod]
    public async Task RunAsync_BadRows_AreRemovedAndReported()
    {
        string csv = SampleData.Csv(
            "Ada,Lovelace,ada@example.com,99201",
            "Bad,Email,not-an-email,99201",
            "Bad,Zip,bz@example.com,9920",
            "Bad,Zip,bz2@example.com,ABCDE",
            "Grace,Hopper,grace@example.com,98101");
        using var api = new FakeMayhemApi { People = (_, _) => Responses.Ok(csv) };
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();
        string output = temp.PathOf("out.csv");

        EnrichmentReport report = await CreateEnricher(connection.Client).RunAsync(SampleData.User, output, testContext.CancellationToken);

        string[] lines = await File.ReadAllLinesAsync(output, testContext.CancellationToken);
        Assert.HasCount(3, lines);
        Assert.AreEqual(5, report.RowsDownloaded);
        CollectionAssert.AreEqual(
            new[] { new RejectionCount(RejectionReasons.InvalidZipCode, 2), new RejectionCount(RejectionReasons.InvalidEmail, 1) },
            report.RejectionCounts.ToArray());

        // Invalid people are never looked up.
        Assert.AreEqual(0, api.AttemptsFor(SampleData.ZipPath("9920")));
        Assert.AreEqual(0, api.AttemptsFor(SampleData.ZipPath("ABCDE")));
    }

    // ---- Location endpoint failure modes --------------------------------------------------

    [TestMethod]
    public async Task RunAsync_LookupFailsThenRecovers_EveryoneIsEnriched()
    {
        using var api = new FakeMayhemApi
        {
            Zip = (zip, attempt, token) => attempt switch
            {
                1 => Responses.ServerError(),
                2 => Responses.InvalidData(),
                3 => Responses.Hang(token),
                _ => Responses.Ok(SampleData.LocationBodyFor(zip)),
            },
        };
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();
        string output = temp.PathOf("out.csv");

        EnrichmentReport report = await CreateEnricher(connection.Client, maxAttempts: 4).RunAsync(SampleData.User, output, testContext.CancellationToken);

        Assert.IsEmpty(report.Rejections);
        Assert.AreEqual(4, api.AttemptsFor(SampleData.ZipPath("99201")));
        CollectionAssert.AreEqual(expectedOutput, await File.ReadAllLinesAsync(output, testContext.CancellationToken));
        Assert.IsTrue(api.AllBodyStreamsDisposed);
    }

    [TestMethod]
    public async Task RunAsync_LookupAlwaysErrors_RemovesOnlyThatPerson()
    {
        using var api = new FakeMayhemApi
        {
            Zip = (zip, _, _) => zip == "98101" ? Responses.ServerError() : Responses.Ok(SampleData.LocationBodyFor(zip)),
        };

        await AssertGraceIsRemovedAsync(api, RejectionReasons.LookupHttpError);
    }

    [TestMethod]
    public async Task RunAsync_LookupAlwaysInvalid_RemovesOnlyThatPerson()
    {
        using var api = new FakeMayhemApi
        {
            Zip = (zip, _, _) => zip == "98101" ? Responses.InvalidData() : Responses.Ok(SampleData.LocationBodyFor(zip)),
        };

        await AssertGraceIsRemovedAsync(api, RejectionReasons.LookupInvalidData);
    }

    [TestMethod]
    public async Task RunAsync_LookupAlwaysTimesOut_RemovesOnlyThatPerson()
    {
        using var api = new FakeMayhemApi
        {
            Zip = (zip, _, token) => zip == "98101" ? Responses.Hang(token) : Responses.Ok(SampleData.LocationBodyFor(zip)),
        };

        await AssertGraceIsRemovedAsync(api, RejectionReasons.LookupTimedOut);
    }

    [TestMethod]
    public async Task RunAsync_LookupReturnsBadState_RemovesThatPerson()
    {
        using var api = new FakeMayhemApi
        {
            Zip = (zip, _, _) => Responses.Ok(zip == "98101" ? "\"SEATTLE\",\"ZZ\"" : SampleData.LocationBodyFor(zip)),
        };

        await AssertGraceIsRemovedAsync(api, RejectionReasons.LookupInvalidData);
    }

    // ---- Concurrency ----------------------------------------------------------------------

    [TestMethod]
    public async Task EnrichAsync_LooksUpConcurrently_ButNeverExceedsTheLimit()
    {
        const int limit = 4;
        using var api = new FakeMayhemApi
        {
            People = (_, _) => Responses.Ok(SampleData.ManyPeopleCsv(40)),
            Zip = async (_, _, token) =>
            {
                await Task.Delay(20, token);
                return await Responses.Ok("\"SPOKANE\",\"WA\"");
            },
        };
        using FakeConnection connection = api.Connect();

        EnrichmentReport report = await CreateEnricher(connection.Client, maxConcurrentLookups: limit).EnrichAsync(SampleData.User, testContext.CancellationToken);

        Assert.HasCount(40, report.Enriched);
        Assert.AreEqual(limit, api.MaxInFlight);
    }

    // ---- Cancellation ---------------------------------------------------------------------

    [TestMethod]
    public async Task RunAsync_CancelledDuringLookups_ThrowsAndWritesNothing()
    {
        using var api = new FakeMayhemApi { Zip = (_, _, token) => Responses.Hang(token) };
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(testContext.CancellationToken);
        cancellation.CancelAfter(TimeSpan.FromMilliseconds(100));

        // Long attempt timeout so that only the caller's cancellation can stop the lookups.
        PeopleEnricher enricher = CreateEnricher(connection.Client, attemptTimeout: TimeSpan.FromMinutes(1));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => enricher.RunAsync(SampleData.User, temp.PathOf("out.csv"), cancellation.Token));

        Assert.IsEmpty(temp.Files());
        Assert.IsTrue(api.AllBodyStreamsDisposed);
    }

    [TestMethod]
    public async Task RunAsync_CancelledDuringPeopleDownload_ThrowsAndWritesNothing()
    {
        using var api = new FakeMayhemApi { People = (_, token) => Responses.Hang(token) };
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(testContext.CancellationToken);
        cancellation.CancelAfter(TimeSpan.FromMilliseconds(100));
        PeopleEnricher enricher = CreateEnricher(connection.Client, attemptTimeout: TimeSpan.FromMinutes(1));

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => enricher.RunAsync(SampleData.User, temp.PathOf("out.csv"), cancellation.Token));

        Assert.AreEqual(1, api.AttemptsFor(SampleData.PeoplePath));
        Assert.IsEmpty(temp.Files());
    }

    [TestMethod]
    public async Task RunAsync_AlreadyCancelled_MakesNoRequests()
    {
        using var api = new FakeMayhemApi();
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => CreateEnricher(connection.Client).RunAsync(SampleData.User, temp.PathOf("out.csv"), cancellation.Token));

        Assert.IsEmpty(api.RequestUris);
        Assert.IsEmpty(temp.Files());
    }

    // ---- Unexpected exceptions ------------------------------------------------------------

    [TestMethod]
    public async Task RunAsync_UnexpectedExceptionDuringLookup_PropagatesAndWritesNothing()
    {
        var bug = new InvalidOperationException("bug");
        using var api = new FakeMayhemApi { Zip = (_, _, _) => throw bug };
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();

        InvalidOperationException thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            () => CreateEnricher(connection.Client).RunAsync(SampleData.User, temp.PathOf("out.csv"), testContext.CancellationToken));

        Assert.AreSame(bug, thrown);
        Assert.AreEqual(1, api.AttemptsFor(SampleData.ZipPath("99201")), "Unexpected exceptions must not be retried.");
        Assert.IsEmpty(temp.Files());
    }

    // ---- Resource cleanup -----------------------------------------------------------------

    [TestMethod]
    public async Task RunAsync_MixedFailures_DisposesEveryResponseStream()
    {
        using var api = new FakeMayhemApi
        {
            People = (attempt, _) => attempt == 1 ? Responses.ServerError() : Responses.Ok(SampleData.PeopleCsv),
            Zip = (zip, attempt, _) => zip switch
            {
                "99201" => Responses.ServerError(),
                "98101" => Responses.InvalidData(),
                _ => attempt == 1 ? Responses.ServerError() : Responses.Ok(SampleData.LocationBodyFor(zip)),
            },
        };
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();

        EnrichmentReport report = await CreateEnricher(connection.Client).RunAsync(SampleData.User, temp.PathOf("out.csv"), testContext.CancellationToken);

        Assert.HasCount(1, report.Enriched);
        Assert.HasCount(2, report.Rejections);
        Assert.IsGreaterThanOrEqualTo(9, api.ResponseCount, $"Expected at least 9 responses but saw {api.ResponseCount}.");
        Assert.IsGreaterThan(0, api.BodyStreamCount, "Successful and invalid-data responses should have been read.");
        Assert.IsTrue(api.AllBodyStreamsDisposed);
    }

    // ---- Helpers --------------------------------------------------------------------------

    private static PeopleEnricher CreateEnricher(
        MayhemMarmotClient client,
        int maxAttempts = 3,
        int maxConcurrentLookups = 4,
        TimeSpan? attemptTimeout = null) =>
        new(client, TestRetry.Create(maxAttempts, attemptTimeout), maxConcurrentLookups);

    private async Task AssertPeopleDownloadFailsAsync<TException>(FakeMayhemApi api)
        where TException : Exception
    {
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();
        string output = temp.PathOf("out.csv");
        await File.WriteAllTextAsync(output, "previous run", testContext.CancellationToken);

        await Assert.ThrowsExactlyAsync<TException>(
            () => CreateEnricher(connection.Client, maxAttempts: 3).RunAsync(SampleData.User, output, testContext.CancellationToken));

        Assert.AreEqual(3, api.AttemptsFor(SampleData.PeoplePath));
        Assert.AreEqual(0, api.RequestUris.Count(uri => uri.AbsolutePath.StartsWith("/people/zip/", StringComparison.Ordinal)));
        CollectionAssert.AreEqual(new[] { output }, temp.Files(), "No temp or partial files may be left behind.");
        Assert.AreEqual("previous run", await File.ReadAllTextAsync(output, testContext.CancellationToken));
        Assert.IsTrue(api.AllBodyStreamsDisposed);
    }

    private async Task AssertGraceIsRemovedAsync(FakeMayhemApi api, string expectedReason)
    {
        using FakeConnection connection = api.Connect();
        using var temp = new TempDirectory();
        string output = temp.PathOf("out.csv");

        EnrichmentReport report = await CreateEnricher(connection.Client).RunAsync(SampleData.User, output, testContext.CancellationToken);

        CollectionAssert.AreEqual(
            new[] { expectedOutput[0], expectedOutput[1], expectedOutput[3] },
            await File.ReadAllLinesAsync(output, testContext.CancellationToken));

        Rejection rejection = report.Rejections.Single();
        Assert.AreEqual(expectedReason, rejection.Reason);
        Assert.Contains("Grace", rejection.Subject);
        Assert.AreEqual(3, api.AttemptsFor(SampleData.ZipPath("98101")), "The lookup should be retried before giving up.");
        Assert.IsTrue(api.AllBodyStreamsDisposed);
    }
}
