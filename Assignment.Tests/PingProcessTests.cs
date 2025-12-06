using IntelliTect.TestTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;


namespace Assignment.Tests;

[TestClass]
public class PingProcessTests
{
    PingProcessMock Sut { get; set; } = new();

    [TestInitialize]
    public void TestInitialize()
    {
        Sut = new();
    }

    [TestMethod]
    public void Run_GoogleDotCom_Success()
    {
        int exitCode = Sut.Run("google.com").ExitCode;
        Assert.AreEqual<int>(0, exitCode);
    }


    [TestMethod]
    public void Run_InvalidAddressOutput_Success()
    {
        (int exitCode, string? stdOutput) = Sut.Run("badaddress");
        Assert.IsFalse(string.IsNullOrWhiteSpace(stdOutput));
        stdOutput = WildcardPattern.NormalizeLineEndings(stdOutput!.Trim());
        Assert.AreEqual<string?>(
            "Ping request could not find host badaddress. Please check the name and try again.".Trim(),
            stdOutput,
            $"Output is unexpected: {stdOutput}");
        Assert.AreEqual<int>(1, exitCode);
    }

    [TestMethod]
    public void Run_CaptureStdOutput_Success()
    {
        PingResult result = Sut.Run("localhost");
        AssertValidPingOutput(result);
    }

    [TestMethod]
    public void RunTaskAsync_Success()
    {
        Task<PingResult> task = Sut.RunTaskAsync("localhost");
        PingResult result = task.Result;
        AssertValidPingOutput(result);
    }

    [TestMethod]
    public void RunAsync_UsingTaskReturn_Success()
    {
        PingResult result = Sut.RunAsync("localhost").Result;
        AssertValidPingOutput(result);
    }

    [TestMethod]
    async public Task RunAsync_UsingTpl_Success()
    {
        PingResult result = await Sut.RunAsync("localhost");
        AssertValidPingOutput(result);
    }

    [TestMethod]
    public async Task RunAsync_UsingTplWithCancellation_TaskCanceledException()
    {
        CancellationTokenSource cts = new();
        Task<PingResult> task = Sut.RunAsync("localhost", cts.Token);
        cts.Cancel();

        try
        {
            await task;
            Assert.Fail("Expected OperationCanceledException to be thrown");
        }
        catch (OperationCanceledException)
        {
            // Expected exception
        }
    }




    [TestMethod]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrapping()
    {
        CancellationTokenSource cts = new();
        Task<PingResult> task = Sut.RunAsync("localhost", cts.Token);

        cts.Cancel();

        try
        {
            task.Wait();
            Assert.Fail("Expected TaskCanceledException wrapped in AggregateException");
        }
        catch (AggregateException ex)
        {
            ex = ex.Flatten();

            Assert.IsTrue(
                ex.InnerExceptions.Any(e => e is TaskCanceledException),
                "Expected a TaskCanceledException inside AggregateException.");
        }
    }

    [TestMethod]
    async public Task RunAsync_MultipleHostAddresses_True()
    {
        string[] hostNames = new[] { "localhost", "localhost", "localhost", "localhost" };

        int expectedLineCount = PingOutputLikeExpression.Split(Environment.NewLine).Length * hostNames.Length;

        PingResult result = await Sut.RunAsync(hostNames);

        int? lineCount = result.StdOutput?.Split(Environment.NewLine, StringSplitOptions.None).Length;

        Assert.AreEqual(expectedLineCount, lineCount);
    }

    [TestMethod]
    public async Task RunAsync_MultipleHostAddresses_WithCancellation_ThrowsTaskCanceledException()
    {
        string[] hostNames = new[] { "localhost", "localhost", "localhost", "localhost" };
        CancellationTokenSource cts = new();
        cts.Cancel();

        Task<PingResult> task = Sut.RunAsync(hostNames, cts.Token);

        await Assert.ThrowsExactlyAsync<TaskCanceledException>(async () => await task);
    }

    [TestMethod]
    async public Task RunLongRunningAsync_UsingTpl_Success()
    {
        PingResult result = await Sut.RunLongRunningAsync("localhost");
        AssertValidPingOutput(result);
    }

    [TestMethod]
    public void RunLongRunningAsync_WithCallbacksAndStartInfo_Success()
    {
        ProcessStartInfo startInfo = new("ping")
        {
            Arguments = "localhost",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        List<string?> output = new();
        int exitCode = Sut.RunLongRunningAsync(startInfo, line => output.Add(line), null!, CancellationToken.None).Result;
        string stdOutput = string.Join(Environment.NewLine, output.Where(l => l is not null));
        AssertValidPingOutput(exitCode, stdOutput);
    }

    [TestMethod]
    public void RunLongRunningAsync_WithCancellation_AggregateException()
    {
        ProcessStartInfo startInfo = new("ping")
        {
            Arguments = "localhost",
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        using CancellationTokenSource cts = new();
        Task<int> task = Sut.RunLongRunningAsync(startInfo, _ => { }, null!, cts.Token);
        cts.Cancel();

        Assert.Throws<AggregateException>(() => { _ = task.Result; });
    }

    [TestMethod]
    public async Task RunAsync_WithProgress_ReportsLines_Success()
    {
        List<string?> lines = new();
        ManualResetEventSlim completed = new(false);
        Progress<string?> progress = new(line =>
        {
            lines.Add(line);
            if (line == null)
            {
                completed.Set();
            }
        });

        PingResult result = await Sut.RunAsync("localhost", progress);

        // Wait for progress to complete reporting
        completed.Wait(TimeSpan.FromSeconds(5));

        IEnumerable<string?> nonEmptyLines = lines.Where(l => !string.IsNullOrWhiteSpace(l));
        Assert.IsNotEmpty(nonEmptyLines, "Progress should have reported non-empty lines");
        AssertValidPingOutput(result);
    }

    [TestMethod]
    public void StringBuilderAppendLine_InParallel_IsNotThreadSafe()
    {
        IEnumerable<int> numbers = Enumerable.Range(0, short.MaxValue);
        System.Text.StringBuilder stringBuilder = new();

        try
        {
            numbers.AsParallel().ForAll(item => stringBuilder.AppendLine(""));
            int lineCount = stringBuilder.ToString().Split(Environment.NewLine, StringSplitOptions.None).Length;
            Assert.AreNotEqual(lineCount, numbers.Count() + 1,
                "StringBuilder should not be thread-safe - line count should be incorrect due to race conditions");
        }
        catch (AggregateException)
        {
            // Expected: StringBuilder throws exceptions when used in parallel due to thread safety issues
            return;
        }
    }

    readonly string PingOutputLikeExpression = @"
Pinging * with 32 bytes of data:
Reply from ::1: time<*
Reply from ::1: time<*
Reply from ::1: time<*
Reply from ::1: time<*

Ping statistics for ::1:
    Packets: Sent = *, Received = *, Lost = 0 (0% loss),
Approximate round trip times in milli-seconds:
    Minimum = *, Maximum = *, Average = *".Trim();
    private void AssertValidPingOutput(int exitCode, string? stdOutput)
    {
        Assert.IsFalse(string.IsNullOrWhiteSpace(stdOutput));
        stdOutput = WildcardPattern.NormalizeLineEndings(stdOutput!.Trim());
        Assert.IsTrue(stdOutput?.IsLike(PingOutputLikeExpression) ?? false,
            $"Output is unexpected: {stdOutput}");
        Assert.AreEqual<int>(0, exitCode);
    }
    private void AssertValidPingOutput(PingResult result) =>
        AssertValidPingOutput(result.ExitCode, result.StdOutput);
}
