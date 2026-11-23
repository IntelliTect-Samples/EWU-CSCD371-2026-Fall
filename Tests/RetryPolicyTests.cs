using System.Net;
using Microsoft.Kiota.Abstractions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment9.Tests;

[TestClass]
public sealed class RetryPolicyTests(TestContext testContext)
{
    [TestMethod]
    public async Task ExecuteAsync_FirstAttemptSucceeds_RunsOnce()
    {
        int attempts = 0;

        int result = await TestRetry.Create().ExecuteAsync(
            _ =>
            {
                attempts++;
                return Task.FromResult(42);
            },
            testContext.CancellationToken);

        Assert.AreEqual(42, result);
        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public async Task ExecuteAsync_HttpError_RetriesUntilSuccess()
    {
        int attempts = 0;

        int result = await TestRetry.Create(maxAttempts: 3).ExecuteAsync(
            _ => ++attempts < 3
                ? throw new HttpRequestException("boom", null, HttpStatusCode.InternalServerError)
                : Task.FromResult(attempts),
            testContext.CancellationToken);

        Assert.AreEqual(3, result);
    }

    [TestMethod]
    public async Task ExecuteAsync_GeneratedClientErrorStatus_IsRetried()
    {
        int attempts = 0;

        string result = await TestRetry.Create(maxAttempts: 2).ExecuteAsync(
            _ => ++attempts == 1
                ? throw new ApiException("500 from the server") { ResponseStatusCode = 500 }
                : Task.FromResult("ok"),
            testContext.CancellationToken);

        Assert.AreEqual("ok", result);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task ExecuteAsync_InvalidData_IsRetried()
    {
        int attempts = 0;

        string result = await TestRetry.Create(maxAttempts: 2).ExecuteAsync(
            _ => ++attempts == 1 ? throw new InvalidResponseException("BAD_DATA") : Task.FromResult("ok"),
            testContext.CancellationToken);

        Assert.AreEqual("ok", result);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task ExecuteAsync_TruncatedBody_IsRetried()
    {
        int attempts = 0;

        string result = await TestRetry.Create(maxAttempts: 2).ExecuteAsync(
            _ => ++attempts == 1
                ? throw new HttpIOException(HttpRequestError.ResponseEnded, "cut off")
                : Task.FromResult("ok"),
            testContext.CancellationToken);

        Assert.AreEqual("ok", result);
    }

    [TestMethod]
    public async Task ExecuteAsync_AttemptTimesOut_IsRetried()
    {
        int attempts = 0;

        string result = await TestRetry.Create(maxAttempts: 2, attemptTimeout: TimeSpan.FromMilliseconds(50)).ExecuteAsync(
            async token =>
            {
                if (++attempts == 1)
                {
                    await Task.Delay(Timeout.Infinite, token);
                }

                return "ok";
            },
            testContext.CancellationToken);

        Assert.AreEqual("ok", result);
        Assert.AreEqual(2, attempts);
    }

    [TestMethod]
    public async Task ExecuteAsync_EveryAttemptTimesOut_ThrowsTimeoutException()
    {
        int attempts = 0;
        RetryPolicy policy = TestRetry.Create(maxAttempts: 3, attemptTimeout: TimeSpan.FromMilliseconds(30));

        TimeoutException exception = await Assert.ThrowsExactlyAsync<TimeoutException>(() => policy.ExecuteAsync<string>(
            async token =>
            {
                attempts++;
                await Task.Delay(Timeout.Infinite, token);
                return "never";
            },
            testContext.CancellationToken));

        Assert.AreEqual(3, attempts);
        Assert.IsInstanceOfType<OperationCanceledException>(exception.InnerException);
    }

    [TestMethod]
    public async Task ExecuteAsync_EveryAttemptFails_ThrowsLastExceptionUnchanged()
    {
        int attempts = 0;
        HttpRequestException? last = null;

        HttpRequestException thrown = await Assert.ThrowsExactlyAsync<HttpRequestException>(() =>
            TestRetry.Create(maxAttempts: 4).ExecuteAsync<int>(
                _ =>
                {
                    attempts++;
                    last = new HttpRequestException($"attempt {attempts}");
                    throw last;
                },
                testContext.CancellationToken));

        Assert.AreEqual(4, attempts);
        Assert.AreSame(last, thrown);
    }

    [TestMethod]
    public async Task ExecuteAsync_UnexpectedException_PropagatesImmediatelyWithOriginalStackTrace()
    {
        int attempts = 0;
        var bug = new InvalidOperationException("bug");

        InvalidOperationException thrown = await Assert.ThrowsExactlyAsync<InvalidOperationException>(() =>
            TestRetry.Create(maxAttempts: 5).ExecuteAsync<int>(_ => ThrowBugAsync(bug, ref attempts), testContext.CancellationToken));

        Assert.AreSame(bug, thrown);
        Assert.AreEqual(1, attempts);
        Assert.IsNotNull(thrown.StackTrace);
        Assert.Contains(nameof(ThrowBugAsync), thrown.StackTrace);
    }

    [TestMethod]
    public async Task ExecuteAsync_CallerCancels_StopsWithoutRetrying()
    {
        using var cancellation = CancellationTokenSource.CreateLinkedTokenSource(testContext.CancellationToken);
        int attempts = 0;

        await Assert.ThrowsAsync<OperationCanceledException>(() => TestRetry.Create(maxAttempts: 5).ExecuteAsync<int>(
            async token =>
            {
                attempts++;
                await cancellation.CancelAsync();
                await Task.Delay(Timeout.Infinite, token);
                return 0;
            },
            cancellation.Token));

        Assert.AreEqual(1, attempts);
    }

    [TestMethod]
    public async Task ExecuteAsync_AlreadyCancelled_NeverRuns()
    {
        using var cancellation = new CancellationTokenSource();
        await cancellation.CancelAsync();
        int attempts = 0;

        await Assert.ThrowsAsync<OperationCanceledException>(() => TestRetry.Create().ExecuteAsync(
            _ =>
            {
                attempts++;
                return Task.FromResult(0);
            },
            cancellation.Token));

        Assert.AreEqual(0, attempts);
    }

    [TestMethod]
    public void GetDelay_GrowsExponentiallyUpToTheCap()
    {
        var policy = new RetryPolicy(
            new RetryOptions { BaseDelay = TimeSpan.FromMilliseconds(100), MaxDelay = TimeSpan.FromMilliseconds(500) },
            nextRandom: () => 1.0);

        Assert.AreEqual(TimeSpan.FromMilliseconds(100), policy.GetDelay(1));
        Assert.AreEqual(TimeSpan.FromMilliseconds(200), policy.GetDelay(2));
        Assert.AreEqual(TimeSpan.FromMilliseconds(400), policy.GetDelay(3));
        Assert.AreEqual(TimeSpan.FromMilliseconds(500), policy.GetDelay(4));
        Assert.AreEqual(TimeSpan.FromMilliseconds(500), policy.GetDelay(30));
    }

    [TestMethod]
    public void GetDelay_AppliesFullJitter()
    {
        var policy = new RetryPolicy(
            new RetryOptions { BaseDelay = TimeSpan.FromMilliseconds(100), MaxDelay = TimeSpan.FromSeconds(10) },
            nextRandom: () => 0.25);

        Assert.AreEqual(TimeSpan.FromMilliseconds(25), policy.GetDelay(1));
        Assert.AreEqual(TimeSpan.FromMilliseconds(100), policy.GetDelay(3));
    }

    [TestMethod]
    public void GetDelay_WithRealRandom_StaysWithinBounds()
    {
        var policy = new RetryPolicy(new RetryOptions
        {
            BaseDelay = TimeSpan.FromMilliseconds(100),
            MaxDelay = TimeSpan.FromMilliseconds(800),
        });

        TimeSpan[] delays = [.. Enumerable.Range(0, 200).Select(_ => policy.GetDelay(3))];

        Assert.IsTrue(delays.All(delay => delay >= TimeSpan.Zero && delay <= TimeSpan.FromMilliseconds(400)));
        Assert.IsGreaterThan(1, delays.Distinct().Count(), "Jitter should produce varying delays.");
    }

    [TestMethod]
    public void Constructor_InvalidMaxAttempts_Throws()
    {
        Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RetryPolicy(new RetryOptions { MaxAttempts = 0 }));
    }

    private static Task<int> ThrowBugAsync(InvalidOperationException bug, ref int attempts)
    {
        attempts++;
        throw bug;
    }
}
