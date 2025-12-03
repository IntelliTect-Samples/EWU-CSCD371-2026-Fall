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
    PingProcess Sut { get; set; } = new();

    [TestInitialize]
    public void TestInitialize()
    {
        Sut = new();
    }

    [TestMethod]
    public void Start_PingProcess_Success()
    {
        string args = OperatingSystem.IsWindows()
            ? "-n 4 localhost"
            : "-c 4 localhost";

        using Process process = Process.Start("ping", args);
        process!.WaitForExit();
        Assert.AreEqual<int>(0, process.ExitCode);
    }

    [TestMethod]
    public void Run_Localhost_Success() // redesigned not to use google, suspected pipeline issue
    {
        int exitCode = Sut.Run("localhost").ExitCode;
        Assert.AreEqual<int>(0, exitCode);
    }

    [TestMethod]
    public void Run_InvalidAddressOutput_Success()
    {
        (int exitCode, string? stdOutput) = Sut.Run("badaddress");

        Assert.AreNotEqual<int>(0, exitCode);
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
        // Do NOT use async/await in this test.
        Task<PingResult> task = Sut.RunTaskAsync("localhost"); // uses .RunTaskAsync()

        PingResult result = task.GetAwaiter().GetResult();

        AssertValidPingOutput(result);
    }

    [TestMethod]
    public void RunAsync_UsingTaskReturn_Success()
    {
        // Do NOT use async/await in this test.
        Task<PingResult> task = Sut.RunAsync("localhost"); // uses .RunAsync()
        
        PingResult result = task.GetAwaiter().GetResult();
        
        AssertValidPingOutput(result);
    }

    [TestMethod]
    async public Task RunAsync_UsingTpl_Success()
    {
        // DO use async/await in this test.
        PingResult result = await Sut.RunAsync("localhost");

        AssertValidPingOutput(result);
    }

    [TestMethod]
    // [ExpectedException(typeof(AggregateException))]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrapping()
    {
        var cts = new CancellationTokenSource();

        Task<PingResult> task = Sut.RunAsync("localhost", cts.Token);

        cts.Cancel();

        Assert.Throws<AggregateException>(() => task.Wait());
    }

    [TestMethod]
    // [ExpectedException(typeof(TaskCanceledException))]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrappingTaskCanceledException()
    {
        var cts = new CancellationTokenSource();
        Task<PingResult> task = Sut.RunAsync("localhost", cts.Token);

        cts.Cancel();

        var aggregateEx = Assert.Throws<AggregateException>(() => task.Wait());

        Assert.IsTrue(aggregateEx.Flatten().InnerExceptions.Any(e => e is TaskCanceledException),
            "The AggregateException did not contain a TaskCanceledException.");
    }

    [TestMethod]
    async public Task RunAsync_MultipleHostAddresses_True()
    {
        string[] hostNames = new string[] { "localhost", "localhost", "localhost", "localhost" };

        int pingOutputLines = PingOutputLikeExpression.Split(Environment.NewLine).Length;
        int expectedLineCount = pingOutputLines * hostNames.Length;

        PingResult result = await Sut.RunAsync(hostNames);

        Assert.AreEqual(0, result.ExitCode);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result.StdOutput));

        int? lineCount = result.StdOutput?.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length;

        int expectedNonEmptyLineCount = PingOutputLikeExpression.Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries).Length * hostNames.Length;

        Assert.IsNotNull(lineCount, "Standard output should not be null.");

        Assert.IsGreaterThanOrEqualTo(lineCount.Value, expectedLineCount, $"Expected at least {expectedLineCount} lines, but got {lineCount.Value}.");
    }

    [TestMethod]
    async public Task RunLongRunningAsync_UsingTpl_Success()
    {
        PingResult result = await Sut.RunLongRunningAsync("localhost");
        // Test Sut.RunLongRunningAsync("localhost");
        AssertValidPingOutput(result);
    }

    [TestMethod]
    public void StringBuilderAppendLine_InParallel_IsNotThreadSafe()
    {
        const int N = 100_000;
        System.Text.StringBuilder stringBuilder = new();

        int expectedThreadSafeCount = N + 1;

        try
        {
            Parallel.For(0, N, (i) =>
            {
                stringBuilder.AppendLine("");
                Thread.Yield();
            });
        }
        catch (AggregateException ex) when (ex.Flatten().InnerExceptions.Any(e => e is ArgumentException))
        {
            Assert.AreNotEqual(0, expectedThreadSafeCount, "Hard crash (ArgumentException) occurred, proving non-thread-safety.");
            return;
        }

        int lineCount = stringBuilder.ToString().Split(Environment.NewLine).Length;

        Assert.AreNotEqual(expectedThreadSafeCount, lineCount,
            $"Error: StringBuilder was unexpectedly thread-safe. Count was {lineCount}, expected was {expectedThreadSafeCount}.");
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

        if (OperatingSystem.IsWindows())
        {
            Assert.IsTrue(
                stdOutput.IsLike(PingOutputLikeExpression),
                $"Output is unexpected: {stdOutput}");
        }
        else // not Windows...?
        {
            StringAssert.Contains(stdOutput, "PING localhost");
            StringAssert.Contains(stdOutput, "0% packet loss");
        }

        Assert.AreEqual<int>(0, exitCode);
    }
    private void AssertValidPingOutput(PingResult result) =>
        AssertValidPingOutput(result.ExitCode, result.StdOutput);
}
