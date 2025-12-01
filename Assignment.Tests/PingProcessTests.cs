using IntelliTect.TestTools;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment.Tests;

[TestClass]
public class PingProcessTests
{
    FakePingProcess Sut { get; set; } = new();

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
        PingResult result = Sut.RunTaskAsync("localhost").Result;
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
    public async Task RunAsync_UsingTplWithCancellation_ThrowsTaskCanceled()
    {
        using var cts = new CancellationTokenSource();
        Task<PingResult> task = Sut.RunAsync("localhost", cts.Token);
        cts.Cancel();

        await Assert.ThrowsExactlyAsync<TaskCanceledException>(
            async () => await task);
    }

    [TestMethod]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrapping()
    {
        using var cts = new CancellationTokenSource();
        Task<PingResult> task = Sut.RunAsync("localhost", cts.Token);
        cts.Cancel();

        var aggregate = Assert.ThrowsExactly<AggregateException>(task.Wait);
        Assert.IsNotNull(aggregate);
    }

    [TestMethod]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrappingTaskCanceledException()
    {
        using var cts = new CancellationTokenSource();
        Task<PingResult> task = Sut.RunAsync("localhost", cts.Token);
        cts.Cancel();

        var aggregate = Assert.ThrowsExactly<AggregateException>(task.Wait);

        var flattened = aggregate.Flatten();
        Assert.HasCount(1, flattened.InnerExceptions);
        Assert.IsInstanceOfType<TaskCanceledException>(flattened.InnerExceptions[0]);
    }

    [TestMethod]
    async public Task RunAsync_MultipleHostAddresses_True()
    {
        string[] hostNames = ["localhost", "localhost", "localhost", "localhost"];
        int expectedLinesPerPing = _PingOutputLikeExpression.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length;
        int expectedLineCount = expectedLinesPerPing * hostNames.Length;

        PingResult result = await Sut.RunAsync(hostNames);
        int actualLineCount = result.StdOutput?
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Length ?? 0;

        Assert.AreEqual(expectedLineCount, actualLineCount);
    }

    [TestMethod]
    async public Task RunLongRunningAsync_UsingTpl_Success()
    {
        // 1. Arrange
        ProcessStartInfo startInfo = new("ping", "localhost");

        StringBuilder outputBuilder = new();
        StringBuilder errorBuilder = new();

        void ProgressOutput(string? line)
        {
            if (line is not null)
            {
                outputBuilder.AppendLine(line);
            }
        }

        void ProgressError(string? line)
        {
            if (line is not null)
            {
                errorBuilder.AppendLine(line);
            }
        }

        using CancellationTokenSource cts = new();

        // 2. Act
        int exitCode = await Sut.RunLongRunningAsync(startInfo, ProgressOutput, ProgressError, cts.Token);

        // 3. Assert
        string stdOutput = outputBuilder.ToString();
        AssertValidPingOutput(exitCode, stdOutput); 
        Assert.AreEqual(string.Empty, errorBuilder.ToString().Trim(), "Expected no stderr output");

    }

    [TestMethod]
    [Ignore("StringBuilder using thread-unsafe behavior; nondeterministic and not required by assignment..")]
    public void StringBuilderAppendLine_InParallel_IsNotThreadSafe()
    {
        IEnumerable<int> numbers = Enumerable.Range(0, short.MaxValue);
        System.Text.StringBuilder stringBuilder = new();
        numbers.AsParallel().ForAll(item => stringBuilder.AppendLine(""));
        int lineCount = stringBuilder.ToString().Split(Environment.NewLine).Length;
        Assert.AreNotEqual(lineCount, numbers.Count()+1);
    }

    [TestMethod]
    public async Task RunAsync_WithProgress_CapturesIncrementalOutput()
    {
        TaskCompletionSource completion = new();
        ConcurrentQueue<string> lines = new();
        IProgress<string?> progress = new Progress<string?>(line =>
        {
            if (line is not null)
            {
                lines.Enqueue(line);
            } else
            {
                completion.TrySetResult();
            }
        });

        PingResult result = await Sut.RunAsync("localhost", progress);
        AssertValidPingOutput(result);

        await completion.Task;
        int expectedLinesPerPing = _PingOutputLikeExpression.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries).Length;
        Assert.HasCount(expectedLinesPerPing, lines.Where((line) => line.Length != 0), "Expected progress to receive all output lines.");
    }

    readonly string _PingOutputLikeExpression = @"
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
        Assert.IsTrue(stdOutput?.IsLike(_PingOutputLikeExpression)??false,
            $"Output is unexpected: {stdOutput}");
        Assert.AreEqual<int>(0, exitCode);
    }

    private void AssertValidPingOutput(PingResult result) =>
        AssertValidPingOutput(result.ExitCode, result.StdOutput);
}
