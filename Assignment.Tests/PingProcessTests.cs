using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class PingProcessTests
{
    private PingProcess _pingProcess = null!;

    [TestInitialize]
    public void TestInitialize() => _pingProcess = new PingProcess();

    // Task 1: Test RunTaskAsync without async/await
    [TestMethod]
    [Timeout(10000)] // 10 second timeout
    public void RunTaskAsync_Success()
    {
        const string host = "localhost";

        Task<PingResult> task = _pingProcess.RunTaskAsync(host);
        task.Wait();
        PingResult result = task.Result;

        Assert.AreEqual(0, result.ExitCode);
        Assert.IsNotNull(result.StdOutput);
        Assert.IsTrue(result.StdOutput.Contains("localhost") || result.StdOutput.Contains("127.0.0.1"));
    }

    // Task 2a: Test RunAsync without async/await
    [TestMethod]
    [Timeout(10000)]
    public void RunAsync_UsingTaskReturn_Success()
    {
        const string host = "localhost";

        Task<PingResult> task = _pingProcess.RunAsync(host);
        task.Wait();
        PingResult result = task.Result;

        Assert.AreEqual(0, result.ExitCode);
        Assert.IsNotNull(result.StdOutput);
        Assert.IsTrue(result.StdOutput.Contains("localhost") || result.StdOutput.Contains("127.0.0.1"));
    }

    // Task 2b: Test RunAsync with async/await
    [TestMethod]
    [Timeout(10000)]
    public async Task RunAsync_UsingTpl_Success()
    {
        const string host = "localhost";

        PingResult result = await _pingProcess.RunAsync(host);

        Assert.AreEqual(0, result.ExitCode);
        Assert.IsNotNull(result.StdOutput);
        Assert.IsTrue(result.StdOutput.Contains("localhost") || result.StdOutput.Contains("127.0.0.1"));
    }

    // Task 3a: Test cancellation with AggregateException
    [TestMethod]
    [Timeout(5000)]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrapping()
    {
        var cts = new CancellationTokenSource();

        AggregateException? caughtException = null;
        try
        {
            Task<PingResult> task = _pingProcess.RunAsync("localhost", cts.Token);
            cts.Cancel();
            Thread.Sleep(10);
            task.Wait();
            Assert.Fail("Expected AggregateException to be thrown");
        }
        catch (AggregateException ex)
        {
            caughtException = ex;
        }

        Assert.IsNotNull(caughtException);
        Assert.IsTrue(caughtException.InnerExceptions.Count > 0);
    }

    // Task 3b: Test cancellation with TaskCanceledException inner exception
    [TestMethod]
    [Timeout(5000)]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrappingTaskCanceledException()
    {
        var cts = new CancellationTokenSource();

        AggregateException? caughtException = null;
        try
        {
            Task<PingResult> task = _pingProcess.RunAsync("localhost", cts.Token);
            cts.Cancel();
            Thread.Sleep(10);
            task.Wait();
            Assert.Fail("Expected AggregateException to be thrown");
        }
        catch (AggregateException ex)
        {
            caughtException = ex;
        }

        Assert.IsNotNull(caughtException);
        Assert.IsNotNull(caughtException.InnerException);
        Assert.IsInstanceOfType<OperationCanceledException>(caughtException.InnerException);
    }

    // Task 3c: Test cancellation with async/await
    [TestMethod]
    [Timeout(5000)]
    public async Task RunAsync_UsingTplWithCancellation_CatchTaskCanceledException()
    {
        var cts = new CancellationTokenSource();

        try
        {
            Task<PingResult> task = _pingProcess.RunAsync("localhost", cts.Token);
            cts.Cancel();
            await Task.Delay(10);
            await task;
            Assert.Fail("Expected OperationCanceledException to be thrown");
        }
        catch (OperationCanceledException)
        {
            // Expected
        }
    }

    // Task 4: Test parallel execution
    [TestMethod]
    [Timeout(15000)]
    public async Task RunAsync_MultipleHosts_Success()
    {
        string[] hosts = { "localhost", "localhost", "localhost" };

        PingResult result = await _pingProcess.RunAsync(hosts);

        Assert.AreEqual(0, result.ExitCode);
        Assert.IsNotNull(result.StdOutput);

        int count = 0;
        string output = result.StdOutput.ToLower(CultureInfo.InvariantCulture);
        foreach (string line in output.Split(Environment.NewLine))
        {
            if (line.Contains("localhost") || line.Contains("127.0.0.1"))
            {
                count++;
            }
        }

        Assert.IsTrue(count >= 3, $"Expected at least 3 localhost references, found {count}");
    }

    // Task 4: Test parallel execution with cancellation
    [TestMethod]
    [Timeout(5000)]
    public async Task RunAsync_MultipleHostsWithCancellation_ThrowsException()
    {
        var cts = new CancellationTokenSource();
        string[] hosts = { "localhost", "localhost", "localhost" };

        try
        {
            Task<PingResult> task = _pingProcess.RunAsync(hosts, cts.Token);
            cts.Cancel();
            await Task.Delay(10);
            await task;
            Assert.Fail("Expected OperationCanceledException to be thrown");
        }
        catch (OperationCanceledException)
        {
            // Expected
        }
    }

    // Task 5: Test long running task
    [TestMethod]
    [Timeout(10000)]
    public async Task RunLongRunningAsync_Success()
    {
        const string host = "localhost";

        PingResult result = await _pingProcess.RunLongRunningAsync(host);

        Assert.AreEqual(0, result.ExitCode);
        Assert.IsNotNull(result.StdOutput);
        Assert.IsTrue(result.StdOutput.Contains("localhost") || result.StdOutput.Contains("127.0.0.1"));
    }

    // Task 5: Test long running task with cancellation
    [TestMethod]
    [Timeout(5000)]
    public async Task RunLongRunningAsync_WithCancellation_ThrowsException()
    {
        var cts = new CancellationTokenSource();

        try
        {
            Task<PingResult> task = _pingProcess.RunLongRunningAsync("localhost", cts.Token);
            cts.Cancel();
            await Task.Delay(10);
            await task;
            Assert.Fail("Expected OperationCanceledException");
        }
        catch (OperationCanceledException)
        {
            // Expected
        }
    }

    // Extra Credit: Test IProgress
    [TestMethod]
    [Timeout(10000)]
    public async Task RunAsync_WithProgress_ReportsProgress()
    {
        const string host = "localhost";
        int progressCallCount = 0;

        var progress = new Progress<string>(output =>
        {
            progressCallCount++;
            Assert.IsNotNull(output);
        });

        PingResult result = await _pingProcess.RunAsync(host, progress, CancellationToken.None);

        Assert.AreEqual(0, result.ExitCode);
        Assert.IsNotNull(result.StdOutput);
        Assert.IsTrue(progressCallCount > 0);
    }

    // Additional: Verify StdOutput completeness
    [TestMethod]
    [Timeout(15000)]
    public async Task RunAsync_MultipleHosts_AllOutputCaptured()
    {
        string[] hosts = { "localhost", "localhost" };

        PingResult result = await _pingProcess.RunAsync(hosts);

        Assert.IsNotNull(result.StdOutput);

        string[] lines = result.StdOutput.Split(
            new[] { Environment.NewLine },
            StringSplitOptions.RemoveEmptyEntries);

        Assert.IsTrue(lines.Length > 0, "Output should contain multiple lines");
    }
}
