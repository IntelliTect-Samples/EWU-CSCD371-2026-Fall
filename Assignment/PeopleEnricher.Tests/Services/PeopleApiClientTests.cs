using System.Net;
using PeopleEnricher;

namespace PeopleEnricher.Tests;

[TestClass]
public sealed class PeopleApiClientTests
{
    private const string PeopleCsv =
        "FirstName,LastName,EmailAddress,ZipCode\n" +
        "Jane,Doe,jane@example.com,99201";

    [TestMethod]
    public async Task GetPeopleAsyncReturnsParsedPeople()
    {
        string? requestPath = null;

        StubHttpMessageHandler handler = new((request, _) =>
        {
            requestPath = request.RequestUri?.AbsolutePath;

            return Task.FromResult(CreateResponse(PeopleCsv));
        });

        using HttpClient httpClient = new(handler);

        PeopleApiClient api = new(
            httpClient,
            retryDelay: TimeSpan.Zero);

        PeopleCsvParseResult result =
            await api.GetPeopleAsync("test-user");

        Assert.HasCount(1, result.People);
        Assert.AreEqual("Jane", result.People[0].FirstName);
        Assert.AreEqual("/people/test-user", requestPath);
    }

    [TestMethod]
    public async Task GetZipLocationAsyncReturnsParsedLocation()
    {
        string? requestPath = null;

        StubHttpMessageHandler handler = new((request, _) =>
        {
            requestPath = request.RequestUri?.AbsolutePath;

            return Task.FromResult(
                CreateResponse("\"SPOKANE\",\"WA\""));
        });

        using HttpClient httpClient = new(handler);

        PeopleApiClient api = new(
            httpClient,
            retryDelay: TimeSpan.Zero);

        ZipLocation location =
            await api.GetZipLocationAsync("99201");

        Assert.AreEqual("SPOKANE", location.City);
        Assert.AreEqual("WA", location.State);
        Assert.AreEqual("/people/zip/99201", requestPath);
    }

    [TestMethod]
    public async Task GetZipLocationAsyncRetriesInvalidData()
    {
        int attempts = 0;

        StubHttpMessageHandler handler = new((_, _) =>
        {
            attempts++;

            string response = attempts == 1
                ? "BAD_DATA"
                : "\"SPOKANE\",\"WA\"";

            return Task.FromResult(CreateResponse(response));
        });

        using HttpClient httpClient = new(handler);

        PeopleApiClient api = new(
            httpClient,
            retryDelay: TimeSpan.Zero);

        ZipLocation location =
            await api.GetZipLocationAsync("99201");

        Assert.AreEqual("SPOKANE", location.City);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task GetZipLocationAsyncRetriesHttp418()
    {
        int attempts = 0;

        StubHttpMessageHandler handler = new((_, _) =>
        {
            attempts++;

            HttpResponseMessage response = attempts == 1
                ? CreateResponse(string.Empty, (HttpStatusCode)418)
                : CreateResponse("\"SPOKANE\",\"WA\"");

            return Task.FromResult(response);
        });

        using HttpClient httpClient = new(handler);

        PeopleApiClient api = new(
            httpClient,
            retryDelay: TimeSpan.Zero);

        ZipLocation location =
            await api.GetZipLocationAsync("99201");

        Assert.AreEqual("SPOKANE", location.City);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task GetPeopleAsyncRetriesInterruptedResponse()
    {
        int attempts = 0;

        StubHttpMessageHandler handler = new((_, _) =>
        {
            attempts++;

            if (attempts == 1)
            {
                return Task.FromException<HttpResponseMessage>(
                    new HttpRequestException(
                        "Simulated interrupted response."));
            }

            return Task.FromResult(CreateResponse(PeopleCsv));
        });

        using HttpClient httpClient = new(handler);

        PeopleApiClient api = new(
            httpClient,
            retryDelay: TimeSpan.Zero);

        PeopleCsvParseResult result =
            await api.GetPeopleAsync("test-user");

        Assert.HasCount(1, result.People);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task GetZipLocationAsyncRetriesTimeout()
    {
        int attempts = 0;

        StubHttpMessageHandler handler = new(async (_, token) =>
        {
            attempts++;

            await Task.Delay(Timeout.InfiniteTimeSpan, token);

            return CreateResponse("\"SPOKANE\",\"WA\"");
        });

        using HttpClient httpClient = new(handler);

        PeopleApiClient api = new(
            httpClient,
            maxAttempts: 2,
            requestTimeout: TimeSpan.FromMilliseconds(50),
            retryDelay: TimeSpan.Zero);

        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await api.GetZipLocationAsync("99201"));

        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task GetZipLocationAsyncStopsImmediatelyOnCallerCancellation()
    {
        int attempts = 0;

        TaskCompletionSource<bool> requestStarted = new(
            TaskCreationOptions.RunContinuationsAsynchronously);

        StubHttpMessageHandler handler = new(async (_, token) =>
        {
            attempts++;
            requestStarted.SetResult(true);

            await Task.Delay(Timeout.InfiniteTimeSpan, token);

            return CreateResponse("\"SPOKANE\",\"WA\"");
        });

        using HttpClient httpClient = new(handler);
        using CancellationTokenSource cancellationSource = new();

        PeopleApiClient api = new(
            httpClient,
            retryDelay: TimeSpan.Zero);

        Task<ZipLocation> request = api.GetZipLocationAsync(
            "99201",
            cancellationSource.Token);

        await requestStarted.Task.WaitAsync(
            TimeSpan.FromSeconds(2));

        cancellationSource.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            async () => await request);

        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public async Task GetZipLocationAsyncStopsAfterMaximumAttempts()
    {
        int attempts = 0;

        StubHttpMessageHandler handler = new((_, _) =>
        {
            attempts++;

            return Task.FromResult(CreateResponse("BAD_DATA"));
        });

        using HttpClient httpClient = new(handler);

        PeopleApiClient api = new(
            httpClient,
            maxAttempts: 3,
            retryDelay: TimeSpan.Zero);

        await Assert.ThrowsExactlyAsync<InvalidDataException>(
            async () => await api.GetZipLocationAsync("99201"));

        Assert.AreEqual(3, attempts);
    }

    [TestMethod]
    public async Task GetZipLocationAsyncDoesNotRetryUnexpectedException()
    {
        int attempts = 0;

        StubHttpMessageHandler handler = new((_, _) =>
        {
            attempts++;

            return Task.FromException<HttpResponseMessage>(
                new InvalidOperationException(
                    "Unexpected simulated failure."));
        });

        using HttpClient httpClient = new(handler);

        PeopleApiClient api = new(
            httpClient,
            retryDelay: TimeSpan.Zero);

        await Assert.ThrowsExactlyAsync<InvalidOperationException>(
            async () => await api.GetZipLocationAsync("99201"));

        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public void GetZipLocationAsyncRejectsInvalidZipBeforeRequest()
    {
        int attempts = 0;

        StubHttpMessageHandler handler = new((_, _) =>
        {
            attempts++;
            return Task.FromResult(CreateResponse("\"SPOKANE\",\"WA\""));
        });

        using HttpClient httpClient = new(handler);

        PeopleApiClient api = new(httpClient);

        Assert.ThrowsExactly<ArgumentException>(() =>
        {
            _ = api.GetZipLocationAsync("99A01");
        });

        Assert.AreEqual(0, attempts);
    }

    private static HttpResponseMessage CreateResponse(
        string content,
        HttpStatusCode statusCode = HttpStatusCode.OK)
    {
        return new HttpResponseMessage(statusCode)
        {
            Content = new StringContent(content)
        };
    }

    private sealed class StubHttpMessageHandler(
        Func<HttpRequestMessage, CancellationToken,
            Task<HttpResponseMessage>> sendAsync)
        : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(
            HttpRequestMessage request,
            CancellationToken cancellationToken)
        {
            return sendAsync(request, cancellationToken);
        }
    }
}