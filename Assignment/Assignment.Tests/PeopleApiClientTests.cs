using System.Net;
using static Assignment.Tests.FakeHandler;

namespace Assignment.Tests;

public class PeopleApiClientTests
{
    private const string Spokane = "\"Spokane\",\"WA\"";
    private const string PeopleCsv =
        "FirstName,LastName,EmailAddress,ZipCode\nJohn,Doe,john@example.com,99201";

    private static CancellationToken Token => TestContext.Current.CancellationToken;

    [Theory]
    [InlineData(1, 0, 100)]
    [InlineData(1, 99, 199)]
    [InlineData(2, 0, 200)]
    [InlineData(2, 99, 299)]
    [InlineData(3, 0, 400)]
    [InlineData(3, 99, 499)]
    public void RetryDelayDoublesAndIncludesJitter(int attempt, int jitterMilliseconds, int expected)
    {
        Assert.Equal(expected,
            PeopleApiClient.GetRetryDelayMilliseconds(attempt, jitterMilliseconds));
    }

    // ---------- happy paths ----------

    [Fact]
    public async Task LocationIsParsedFromResponse()
    {
        using var handler = new FakeHandler((_, _, _) => Ok(Spokane));
        using HttpClient http = NewHttp(handler);

        Location location = await new PeopleApiClient(http).GetLocationAsync("99201", Token);

        Assert.Equal(new Location("Spokane", "WA"), location);
        Assert.Equal(1, handler.CountFor("/people/zip/99201"));
    }

    [Fact]
    public async Task PeopleCsvIsReturnedOnceTwoDownloadsMatch()
    {
        using var handler = new FakeHandler((_, _, _) => Ok(PeopleCsv));
        using HttpClient http = NewHttp(handler);

        string csv = await new PeopleApiClient(http).GetPeopleCsvAsync("testuser", Token);

        Assert.Equal(PeopleCsv, csv);
        Assert.Equal(2, handler.CountFor("/people/testuser"));
    }

    [Fact]
    public async Task PeopleCsvWithSwappedZipIsNotTrusted()
    {
        string corrupted = PeopleCsv.Replace("99201", "12345", StringComparison.Ordinal);
        using var handler = new FakeHandler((_, n, _) => Ok(n == 1 ? corrupted : PeopleCsv));
        using HttpClient http = NewHttp(handler);

        string csv = await new PeopleApiClient(http).GetPeopleCsvAsync("testuser", Token);

        Assert.Equal(PeopleCsv, csv);
        Assert.Equal(3, handler.RequestCount);
    }

    // ---------- every failure mode, both endpoints ----------

    [Theory]
    [InlineData("Exception")]
    [InlineData("InvalidData")]
    [InlineData("Timeout")]
    public async Task ZipLookupRetriesEachFailureMode(string mode)
    {
        using var handler = new FakeHandler((_, n, ct) => n == 1 ? Fail(mode, ct) : Ok(Spokane));
        using HttpClient http = NewHttp(handler);

        Location location = await new PeopleApiClient(http).GetLocationAsync("99201", Token);

        Assert.Equal("Spokane", location.City);
        Assert.Equal(2, handler.RequestCount);
    }

    [Theory]
    [InlineData("Exception")]
    [InlineData("InvalidData")]
    [InlineData("Timeout")]
    public async Task PeopleDownloadRetriesEachFailureMode(string mode)
    {
        using var handler = new FakeHandler((_, n, ct) => n == 1 ? Fail(mode, ct) : Ok(PeopleCsv));
        using HttpClient http = NewHttp(handler);

        string csv = await new PeopleApiClient(http).GetPeopleCsvAsync("testuser", Token);

        Assert.Equal(PeopleCsv, csv);
        Assert.Equal(3, handler.RequestCount);   // 1 failure + 2 matching downloads
    }

    [Theory]
    [InlineData("Exception")]
    [InlineData("InvalidData")]
    [InlineData("Timeout")]
    public async Task FailingEveryAttemptThrowsAfterMaxAttempts(string mode)
    {
        using var handler = new FakeHandler((_, _, ct) => Fail(mode, ct));
        using HttpClient http = NewHttp(handler);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => new PeopleApiClient(http).GetLocationAsync("99201", Token));

        Assert.Equal(PeopleApiClient.MaxAttempts, handler.RequestCount);
    }

    [Theory]
    [InlineData(HttpStatusCode.NotFound)]
    [InlineData(HttpStatusCode.BadRequest)]
    public async Task PermanentErrorsAreNotRetried(HttpStatusCode code)
    {
        using var handler = new FakeHandler((_, _, _) => Status(code));
        using HttpClient http = NewHttp(handler);

        await Assert.ThrowsAsync<HttpRequestException>(
            () => new PeopleApiClient(http).GetLocationAsync("99201", Token));

        Assert.Equal(1, handler.RequestCount);
    }

    [Fact]
    public async Task UnexpectedExceptionPropagatesWithoutRetry()
    {
        using var handler = new FakeHandler((_, _, _) => throw new FormatException("bug"));
        using HttpClient http = NewHttp(handler);

        await Assert.ThrowsAsync<FormatException>(
            () => new PeopleApiClient(http).GetLocationAsync("99201", Token));

        Assert.Equal(1, handler.RequestCount);
    }

    // ---------- cancellation ----------

    [Fact]
    public async Task AlreadyCancelledTokenMakesNoRequests()
    {
        using var handler = new FakeHandler((_, _, _) => Ok(Spokane));
        using HttpClient http = NewHttp(handler);
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Token);
        await cts.CancelAsync();

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => new PeopleApiClient(http).GetLocationAsync("99201", cts.Token));

        Assert.Equal(0, handler.RequestCount);
    }

    [Fact]
    public async Task CancellingDuringRequestIsNotRetried()
    {
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(Token);
        using var handler = new FakeHandler((_, _, ct) =>
        {
            cts.Cancel();
            return StallAsync(ct);
        });
        using HttpClient http = NewHttp(handler);

        await Assert.ThrowsAnyAsync<OperationCanceledException>(
            () => new PeopleApiClient(http).GetLocationAsync("99201", cts.Token));

        Assert.Equal(1, handler.RequestCount);
    }
}
