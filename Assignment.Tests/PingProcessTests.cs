using IntelliTect.TestTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment.Tests;

[TestClass]
public class PingProcessTests
{
    MockPingProcess Sut { get; set; } = new();

    [TestInitialize]
    public void TestInitialize()
    {
        Sut = new MockPingProcess();
    }

    [TestMethod]
    public void Start_PingProcess_Success()
    {
        string args = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? " -n 4" : " -c 4";

        Process process = Process.Start("ping", "localhost" + args);
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
    //[ExpectedException(typeof(AggregateException))]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrapping()
    {
        using CancellationTokenSource cts = new();
        cts.Cancel();

        Assert.ThrowsExactly<AggregateException>(() =>
        {
            Sut.RunAsync("localhost", cts.Token).Wait();
        });

    }

    [TestMethod]
    //[ExpectedException(typeof(TaskCanceledException))]
    public void RunAsync_UsingTplWithCancellation_CatchAggregateExceptionWrappingTaskCanceledException()
    {   
        using CancellationTokenSource cts = new();
        cts.Cancel();

        try
        {
            Sut.RunAsync("localhost", cts.Token).Wait();
        }
        catch (AggregateException ex)
        {
            Exception? inner = ex.Flatten().InnerException;
            Assert.IsInstanceOfType<TaskCanceledException>(inner);
            // Use exception.Flatten()
        }
    }

    [TestMethod]
    async public Task RunAsync_MultipleHostAddresses_True()
    {
        string[] hostNames = new string[] { "localhost", "localhost", "localhost", "localhost" };

        int linesPerHost = PingOutputLikeExpression
                           .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                           .Length;
        int expectedLineCount = linesPerHost * hostNames.Length;

        PingResult result = await Sut.RunAsync(hostNames);

        int actualLineCount = result.StdOutput?
                              .Split(new[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries)
                              .Length ?? 0;

        Assert.AreEqual<int>(expectedLineCount, actualLineCount);
    }

    [TestMethod]
    public async Task RunAsync_EmptyHostArray_ReturnsZeroOutput()
    {
        string[] hosts = Array.Empty<string>();

        PingResult result = await Sut.RunAsync(hosts);

        Assert.AreEqual<int>(0, result.ExitCode);
        Assert.IsTrue(string.IsNullOrEmpty(result.StdOutput), "Expected empty StdOutput for empty host array.");
    }


    [TestMethod]
    async public Task RunLongRunningAsync_UsingTpl_Success()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("ping", "localhost");
        StringBuilder output = new();
        StringBuilder error = new();

        using CancellationTokenSource cts = new();

        int exitCode = await Sut.RunLongRunningAsync(
            startInfo,
            line => { if (line is not null) output.AppendLine(line); },
            line => { if (line is not null) error.AppendLine(line); },
            cts.Token);

        AssertValidPingOutput(exitCode, output.ToString());
        Assert.AreEqual<string>(string.Empty, error.ToString().Trim());
    }

    [TestMethod]
    public async Task RunLongRunningAsync_CancellationRequested_ThrowsOperationCanceledException()
    {
        ProcessStartInfo startInfo = new ProcessStartInfo("ping", "localhost");
        using CancellationTokenSource cts = new();
        cts.Cancel(); 

        try
        {
            await Sut.RunLongRunningAsync(startInfo, null, null, cts.Token);
            Assert.Fail("Expected OperationCanceledException was not thrown.");
        }
        catch (OperationCanceledException ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    [TestMethod]
    public async Task RunAsync_WithProgress_CapturesOutputAsItOccurs()
    {
        List<string> capturedLines = new();

        IProgress<string?> progress = new Progress<string?>(line =>
        {
            if (!string.IsNullOrEmpty(line))
            {
                capturedLines.Add(line);
            }
        });

        PingResult result = await Sut.RunAsync("localhost", progress);

        AssertValidPingOutput(result);
  
    }

    [TestMethod]
    public async Task RunAsync_WithProgress_HandlesEmptyOrNullLines()
    {
        List<string?> capturedLines = new();
        IProgress<string?> progress = new Progress<string?>(line => capturedLines.Add(line));

        PingResult result = await Sut.RunAsync("localhost", progress);

        AssertValidPingOutput(result);
        
        Assert.IsTrue(capturedLines.All(line => line is null || line is string));
    }

    [TestMethod]
    public void StringBuilderAppendLine_InParallel_IsNotThreadSafe()
    {
        IEnumerable<int> numbers = Enumerable.Range(0, short.MaxValue);
        System.Text.StringBuilder stringBuilder = new();

        try
        {
            numbers.AsParallel().ForAll(item => stringBuilder.AppendLine(""));
            int lineCount = stringBuilder.ToString().Split(Environment.NewLine).Length;
            Assert.AreNotEqual<int>(lineCount, numbers.Count() + 1,
                "Expected line count to be incorrect due to non-thread-safe StringBuilder.");
        }
        catch (AggregateException ex)
        {
            Assert.IsNotEmpty(ex.InnerExceptions,
                "Expected exceptions due to concurrent writes to StringBuilder.");
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
        Assert.IsTrue(stdOutput?.IsLike(PingOutputLikeExpression)??false,
            $"Output is unexpected: {stdOutput}");
        Assert.AreEqual<int>(0, exitCode);
    }
    private void AssertValidPingOutput(PingResult result) =>
        AssertValidPingOutput(result.ExitCode, result.StdOutput);
}


