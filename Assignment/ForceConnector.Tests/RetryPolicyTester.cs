using ForceConnector.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;
namespace ForceConnector.Tests;

[TestClass]
public class RetryPolicyTester
{
    [TestMethod]
    public async Task ExecuteAsync_SucceedsImmediately()
    {
        RetryPolicy retry = new();

        int calls = 0;

        int result =
            await retry.ExecuteAsync(() =>
            {
                calls++;
                return Task.FromResult(5);
            });

        Assert.AreEqual(1, calls);
        Assert.AreEqual(5, result);
    }
    [TestMethod]
    public async Task ExecuteAsync_RetriesBeforeSuccess()
    {
        RetryPolicy retry = new();

        int calls = 0;

        int result =
            await retry.ExecuteAsync(() =>
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
    public async Task ExecuteAsync_ThrowsAfterMaxAttempts()
    {
        RetryPolicy retry = new();

        object value = await Assert.ThrowsAsync<HttpRequestException>(
                () => retry.ExecuteAsync<int>(() =>
                {
                    throw new HttpRequestException();
                }));
    }
}
