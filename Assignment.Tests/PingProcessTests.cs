using IntelliTect.TestTools;
using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment.Tests;

[TestClass]
[DoNotParallelize]
public class PingProcessTests
{
    private FakePingProcess Sut { get; set; } = new();

    [TestInitialize]
    public void TestInitialize()
    {
        Sut = new FakePingProcess();
    }

    [TestMethod]
    public void Start_PingProcess_Success()
    {
        var psi = OperatingSystem.IsWindows()
            ? new ProcessStartInfo("ping", "127.0.0.1 -n 1")
            : new ProcessStartInfo("ping", "127.0.0.1 -c 1");

        using Process process = Process.Start(psi)!;
        process.WaitForExit();
        Assert.AreEqual(0, process.ExitCode);
    }

    [TestMethod]
    public void Run_InvalidAddressOutput_Success()
    {
        var result = Sut.Run("badaddress");

        Assert.IsFalse(string.IsNullOrWhiteSpace(result.StdOutput));

        string normalized = result.StdOutput!.Trim().ToLowerInvariant();

        // Accept common OS ping error messages
        bool matches = normalized.Contains("temporary failure")
                       || normalized.Contains("name or service not known")
                       || normalized.Contains("could not find host");

        Assert.IsTrue(matches, $"Unexpected output: {result.StdOutput}");
        Assert.AreNotEqual(0, result.ExitCode);
    }

    [TestMethod]
    public void Run_CaptureStdOutput_Success()
    {
        var result = Sut.Run("localhost");
        ValidatePingSuccess(result);
    }

    [TestMethod]
    public void RunTaskAsync_Success()
    {
        var result = Sut.RunTaskAsync("localhost").Result;
        ValidatePingSuccess(result);
    }

    [TestMethod]
    public void RunAsync_UsingTaskReturn_Success()
    {
        var result = Sut.RunAsync("localhost").Result;
        ValidatePingSuccess(result);
    }

    [TestMethod]
    public async Task RunAsync_MultipleHostAddresses_True()
    {
        string[] hosts = { "localhost", "127.0.0.1" };
        var result = await Sut.RunAsync(hosts);
        ValidatePingSuccess(result);
    }

    [TestMethod]
    async public Task RunAsync_UsingTpl_Success()
    {
        var result = await Sut.RunAsync("localhost");
        ValidatePingSuccess(result);
    }

    [TestMethod]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrapping()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var task = Sut.RunAsync("localhost", cts.Token);
        try
        {
            task.Wait();
            Assert.Fail("Expected AggregateException was not thrown.");
        }
        catch (AggregateException ex)
        {
            Assert.IsInstanceOfType<AggregateException>(ex);
        }
    }

    [TestMethod]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrappingTaskCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();
        var task = Sut.RunAsync("localhost", cts.Token);
        try
        {
            task.Wait();
            Assert.Fail("Expected AggregateException was not thrown.");
        }
        catch (AggregateException ex)
        {
            Assert.IsInstanceOfType<TaskCanceledException>(ex.InnerException);
        }
    }

    [TestMethod]
    public void StringBuilderAppendLine_InParallel_DemonstratesNonThreadSafeBehavior()
    {
        var numbers = Enumerable.Range(0, 1000);
        var stringBuilder = new StringBuilder();
        Exception? capturedException = null;

        try
        {
            numbers.AsParallel().ForAll(i => stringBuilder.AppendLine("X"));
        }
        catch (Exception ex)
        {
            capturedException = ex;
        }

        int expectedLength = numbers.Count() * ("X" + Environment.NewLine).Length;
        Assert.IsTrue(
            capturedException != null || stringBuilder.Length != expectedLength,
            "StringBuilder is not thread-safe: either an exception occurs or the content is corrupted."
        );
    }

    [TestMethod]
    public async Task RunLongRunningAsync_ValidPing_ReturnsZero()
    {
        var psi = OperatingSystem.IsWindows()
            ? new ProcessStartInfo("ping", "127.0.0.1 -n 1")
            : new ProcessStartInfo("ping", "127.0.0.1 -c 1");

        int result = await Sut.RunLongRunningAsync(
            psi,
            progressOutput: _ => { },
            progressError: _ => { },
            token: CancellationToken.None);

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public async Task RunLongRunningAsync_OutputProduced_ProgressOutputInvoked()
    {
        int count = 0;
        void output(string? line) { if (line != null) count++; }

        var psi = OperatingSystem.IsWindows()
            ? new ProcessStartInfo("ping", "127.0.0.1 -n 1")
            : new ProcessStartInfo("ping", "127.0.0.1 -c 1");

        await Sut.RunLongRunningAsync(
            psi,
            progressOutput: output,
            progressError: _ => { },
            token: CancellationToken.None);

        Assert.IsGreaterThan(0, count, "Expected progressOutput to be invoked at least once.");
    }

    [TestMethod]
    public void RunLongRunningAsync_Cancelled_ThrowsAggregateException()
    {
        var psi = OperatingSystem.IsWindows()
            ? new ProcessStartInfo("ping", "127.0.0.1 -n 1")
            : new ProcessStartInfo("ping", "127.0.0.1 -c 1");

        using var cts = new CancellationTokenSource();

        var task = Sut.RunLongRunningAsync(
            psi,
            progressOutput: _ => { },
            progressError: _ => { },
            token: cts.Token);

        cts.Cancel();

        try
        {
            task.Wait();
            Assert.Fail("Expected AggregateException was not thrown.");
        }
        catch (AggregateException ex)
        {
            Assert.IsInstanceOfType<TaskCanceledException>(ex.InnerException);
        }
    }

    [TestMethod]
    public async Task RunAsync_WithProgress_CapturesOutputAsItOccurs()
    {
        StringBuilder outputBuilder = new();
        void ProgressHandler(string? line)
        {
            if (line != null)
            {
                outputBuilder.AppendLine(line);
            }
        }

        var result = await Sut.RunAsync("localhost", new Progress<string?>(ProgressHandler), CancellationToken.None);
        ValidatePingSuccess(result);
    }

    // --- Helper for validating ping success across OSes ---
    private static void ValidatePingSuccess(PingResult result)
    {
        Assert.IsNotNull(result.StdOutput);
        Assert.IsGreaterThan(0, result.StdOutput!.Length, "Output should not be empty.");
        Assert.AreEqual(0, result.ExitCode);

        string[] successMarkers = { "Reply from", "bytes from" };
        bool containsMarker = successMarkers.Any(marker =>
            result.StdOutput.Contains(marker, StringComparison.OrdinalIgnoreCase));

        Assert.IsTrue(containsMarker, $"Output did not contain expected markers. Actual output:\n{result.StdOutput}");
    }
}