using ForceConnector.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ForceConnector.Tests;

[TestClass]
public class RetryPolicyTester
{
    [TestMethod]
    public async Task ExecuteAsyncSucceedsImmediately()
    {
        int calls = 0;

        int result =
            await RetryPolicy.ExecuteAsync(() =>
            {
                calls++;
                return Task.FromResult(5);
            });

        Assert.AreEqual(1, calls);
        Assert.AreEqual(5, result);
    }
    [TestMethod]
    public async Task ExecuteAsyncRetriesBeforeSuccess()
    {

        int calls = 0;

        int result =
            await RetryPolicy.ExecuteAsync(() =>
            {
                calls++;

                if (calls < 3)
                {
                    throw new HttpRequestException();
                }

                return Task.FromResult(99);
            });

        Assert.AreEqual(3, calls);
        Assert.AreEqual(99, result);
    }
    [TestMethod]
    public async Task ExecuteAsyncThrowsAfterMaxAttempts()
    {
        int calls = 0;

        await Assert.ThrowsAsync<HttpRequestException>(
                () => RetryPolicy.ExecuteAsync<int>(() =>
                {
                    calls++;
                    throw new HttpRequestException();
                }));

        Assert.AreEqual(3, calls);
    }

    [TestMethod]
    public async Task ExecuteAsyncPropagatesCallerCancellationWithoutRetrying()
    {
        int calls = 0;
        using CancellationTokenSource cancellation = new();
        cancellation.Cancel();

        await Assert.ThrowsAsync<OperationCanceledException>(
            () => RetryPolicy.ExecuteAsync<int>(
                () =>
                {
                    calls++;
                    throw new OperationCanceledException(
                        cancellation.Token);
                },
                cancellation.Token));

        Assert.AreEqual(1, calls);
    }
    [TestMethod]
    public async Task ExecuteAsyncRetriesInvalidDataException()
    {
        int calls = 0;

        await Assert.ThrowsAsync<InvalidDataException>(
            () => RetryPolicy.ExecuteAsync<int>(() =>
            {
                calls++;

                throw new InvalidDataException(
                    "Bad location CSV.");
            }));

        Assert.AreEqual(3, calls);
    }
}
