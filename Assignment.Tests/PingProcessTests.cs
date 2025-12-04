using IntelliTect.TestTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Security;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment.Tests;

[TestClass]
public class PingProcessTests
{
    PingProcess Sut { get; set; } = new ClonePing();

    [TestInitialize]
    public void TestInitialize()
    {
        Sut = new ClonePing();
    }

    [TestMethod]
    public void Start_PingProcess_Success()
    {
        string args = OperatingSystem.IsWindows()
            ? "-n 4 localhost"
            : "-c 4 localhost";
        Process process = Process.Start("ping", args);
        process.WaitForExit();
        Assert.AreEqual<int>(0, process.ExitCode);
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
    public void RunTaskAsync_LocalhostReturnsValidPingOutput_Success()
    {
        Task<PingResult> task = Sut.RunTaskAsync("localhost");
        PingResult result = task.Result;

        AssertValidPingOutput(result);
    }

    [TestMethod]
    public void RunAsync_UsingTaskReturn_Success()
    {

        Task<PingResult> task = Sut.RunAsync("localhost");
        PingResult result = task.Result;


        AssertValidPingOutput(result);
    }

    [TestMethod]
    async public Task RunAsync_UsingTpl_Success()
    {
        Task<PingResult> task = Sut.RunAsync("localhost");
        PingResult result =  await task;
        AssertValidPingOutput(result);
    }


    [TestMethod]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrapping()
    {
        CancellationTokenSource token = new();
        token.Cancel();

        try
        {
            Task<PingResult> task = Sut.RunAsync("localhost", token.Token);
            task.Wait();

        }
        catch (AggregateException ex)
        {

            Assert.IsInstanceOfType<AggregateException>(ex);
            return;

        }

        Assert.Fail("Expected Aggregate Exception, but none was thrown");
        
    }

    [TestMethod]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrappingTaskCanceledException()
    {
        CancellationTokenSource token = new();
        token.Cancel();

        try {
            Task<PingResult> task = Sut.RunAsync("localhost", token.Token);
            task.Wait();

        }
        catch(AggregateException ex) {

            bool exception = ex.Flatten().InnerExceptions.Any(e => e is TaskCanceledException);
            Assert.IsTrue(exception);
            return;
        }

        Assert.Fail("Expected Aggregate Exception, but none was thrown");
    }

    [TestMethod]
    async public Task RunAsync_MultipleHostAddresses_True()
    {
        string[] hostNames = new[] { "localhost", "localhost", "localhost", "localhost" };

        PingResult single = Sut.Run("localhost");
        int linesPerPing = single.StdOutput?
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Length ?? 0;

        int expectedLineCount = linesPerPing * hostNames.Length;

        PingResult result = await Sut.RunAsync(hostNames);
        int actualLineCount = result.StdOutput?
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .Length ?? 0;

        Assert.AreEqual(expectedLineCount, actualLineCount, $"Expected {expectedLineCount} lines but got {actualLineCount}.");
    }

    [TestMethod]
    async public Task RunLongRunningAsync_UsingTpl_Success()
    {
        PingResult result = await Sut.RunLongRunningAsync("localhost");
        AssertValidPingOutput(result);
    }

    [TestMethod]
    public async Task RunLongRunningAsync_WithStartInfo_Success()
    {
        var psi = new ProcessStartInfo("ping")
        {
            Arguments = OperatingSystem.IsWindows()
                ? "-n 4 localhost"
                : "-c 4 localhost",
        };

        int exitCode = await Sut.RunLongRunningAsync(
            psi,
            progressOutput: _ => { },
            progressError: _ => { },
            token: CancellationToken.None);

        Assert.AreEqual<int>(0, exitCode);
    }

    [TestMethod]
    public async Task RunAsync_WithProgress_ReportsOutput()
    {
        List<string?> lines = new();
        IProgress<string?> progress = new Progress<string?>(line =>
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                lines.Add(line);
            }
        });

        PingResult result = await Sut.RunAsync("localhost", progress);

        AssertValidPingOutput(result);
        Assert.AreNotEqual<int>(0, lines.Count, "Expected at least one line reported via progress.");
    }

    [TestMethod]
    [Ignore] // Ignored because this is a test to demonstrate that StringBuilder is not thread-safe.
    public void StringBuilderAppendLine_InParallel_IsNotThreadSafe()
    {
        IEnumerable<int> numbers = Enumerable.Range(0, short.MaxValue);
        System.Text.StringBuilder stringBuilder = new();
        numbers.AsParallel().ForAll(item => stringBuilder.AppendLine(""));
        int lineCount = stringBuilder.ToString().Split(Environment.NewLine).Length;
        Assert.AreNotEqual(lineCount, numbers.Count()+1);
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
      /* if (!OperatingSystem.IsWindows())
        {
            Assert.Inconclusive("PingOutputLikeExpression only matches Windows ping output; skip on non-Windows");
        }*/

        Assert.IsFalse(string.IsNullOrWhiteSpace(stdOutput));

        stdOutput = WildcardPattern.NormalizeLineEndings(stdOutput!.Trim());
        
        Assert.IsTrue(stdOutput?.IsLike(PingOutputLikeExpression)??false,
            $"Output is unexpected: {stdOutput}");

        Assert.AreEqual<int>(0, exitCode);
    }
    private void AssertValidPingOutput(PingResult result) =>
        AssertValidPingOutput(result.ExitCode, result.StdOutput);
}
