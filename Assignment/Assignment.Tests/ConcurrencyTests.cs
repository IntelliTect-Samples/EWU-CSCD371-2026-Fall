using System.Net;
using System.Text;

namespace Assignment.Tests;

[TestClass]
public sealed class ConcurrencyTests
{
    public TestContext TestContext { get; set; } = null!;

    [TestMethod]
    [DataRow(1)]
    [DataRow(3)]
    [DataRow(5)]
    public async Task RunAsyncLooksUpZipsConcurrentlyWithoutExceedingLimit(int limit)
    {
        using ConcurrencyTrackingHandler handler = new(peopleCount: 20, releaseAt: limit);
        using HttpClient http = new(handler) { BaseAddress = new Uri("http://test") };
        PeopleEnricherService service = new(http, maxAttempts: 1, maxConcurrency: limit);
        string output = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid():N}.csv");

        try
        {
            RunSummary summary = await service.RunAsync("tester", output, TestContext.CancellationToken);

            Assert.AreEqual(20, summary.Written);
            // The handler only lets requests finish once `limit` are in flight at the same time,
            // so reaching here proves they ran concurrently; MaxInFlight proves the cap held.
            Assert.AreEqual(limit, handler.MaxInFlight);
        }
        finally
        {
            File.Delete(output);
        }
    }

    [TestMethod]
    [DataRow(0)]
    [DataRow(-1)]
    public void ConstructorThrowsWhenMaxConcurrencyNotPositive(int limit)
    {
        using HttpClient http = new();
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new PeopleEnricherService(http, maxConcurrency: limit));
    }

    /// <summary>
    /// Serves a people CSV, and answers ZIP lookups only after <c>releaseAt</c> of them are
    /// in flight simultaneously, recording the highest number ever in flight.
    /// </summary>
    private sealed class ConcurrencyTrackingHandler(int peopleCount, int releaseAt) : HttpMessageHandler
    {
        private readonly TaskCompletionSource _released = new(TaskCreationOptions.RunContinuationsAsynchronously);
        private int _inFlight;
        private int _maxInFlight;

        public int MaxInFlight => Volatile.Read(ref _maxInFlight);

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            string path = request.RequestUri!.AbsolutePath;

            if (!path.StartsWith("/people/zip/", StringComparison.Ordinal))
            {
                IEnumerable<string> rows = Enumerable.Range(0, peopleCount)
                    .Select(i => $"First{i},Last{i},p{i}@example.com,{99000 + i}");
                return Ok(string.Join('\n', new[] { PeopleEnricherService.PeopleHeader }.Concat(rows)));
            }

            int now = Interlocked.Increment(ref _inFlight);
            UpdateMax(now);
            if (now >= releaseAt)
            {
                _released.TrySetResult();
            }

            try
            {
                // Fails the test (instead of hanging) if requests are being made one at a time.
                await _released.Task.WaitAsync(TimeSpan.FromSeconds(5), cancellationToken);
                await Task.Yield();
                return Ok("\"Spokane\",\"WA\"");
            }
            finally
            {
                Interlocked.Decrement(ref _inFlight);
            }
        }

        private void UpdateMax(int value)
        {
            int current;
            while (value > (current = Volatile.Read(ref _maxInFlight))
                && Interlocked.CompareExchange(ref _maxInFlight, value, current) != current)
            {
            }
        }

        private static HttpResponseMessage Ok(string body) =>
            new(HttpStatusCode.OK) { Content = new StringContent(body, Encoding.UTF8) };
    }
}
