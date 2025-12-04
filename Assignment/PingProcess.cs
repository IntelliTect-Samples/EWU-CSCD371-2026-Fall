using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment;

/// <summary>
/// Represents the result of a ping operation.
/// </summary>
/// <param name="ExitCode">The exit code from the ping process.</param>
/// <param name="StdOutput">The standard output from the ping process.</param>
public record struct PingResult(int ExitCode, string? StdOutput);

/// <summary>
/// Provides methods to execute ping operations synchronously and asynchronously.
/// </summary>
public class PingProcess
{
    private ProcessStartInfo StartInfo { get; } = new("ping");

    /// <summary>
    /// Executes a synchronous ping operation to the specified host.
    /// </summary>
    /// <param name="hostNameOrAddress">The hostname or IP address to ping.</param>
    /// <returns>A <see cref="PingResult"/> containing the exit code and output.</returns>
    public PingResult Run(string hostNameOrAddress)
    {
        StartInfo.Arguments = GetPingArguments(hostNameOrAddress);
        StringBuilder? stringBuilder = null;
        void updateStdOutput(string? line) =>
            (stringBuilder??=new StringBuilder()).AppendLine(line);
        Process process = RunProcessInternal(StartInfo, updateStdOutput, default, default);
        return new PingResult(process.ExitCode, stringBuilder?.ToString());
    }

    /// <summary>
    /// Executes an asynchronous ping operation using Task.Run (without async/await).
    /// </summary>
    /// <param name="hostNameOrAddress">The hostname or IP address to ping.</param>
    /// <returns>A task that represents the asynchronous operation, containing a <see cref="PingResult"/>.</returns>
    public Task<PingResult> RunTaskAsync(string hostNameOrAddress)
    {
        return Task.Run(() => Run(hostNameOrAddress));
    }

    /// <summary>
    /// Executes an asynchronous ping operation without cancellation support.
    /// </summary>
    /// <param name="hostNameOrAddress">The hostname or IP address to ping.</param>
    /// <returns>A task that represents the asynchronous operation, containing a <see cref="PingResult"/>.</returns>
    public Task<PingResult> RunAsync(string hostNameOrAddress)
    {
        return RunAsync(hostNameOrAddress, CancellationToken.None);
    }

    /// <summary>
    /// Executes an asynchronous ping operation with cancellation support.
    /// </summary>
    /// <param name="hostNameOrAddress">The hostname or IP address to ping.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a <see cref="PingResult"/>.</returns>
    /// <exception cref="TaskCanceledException">Thrown when the operation is cancelled.</exception>
    public Task<PingResult> RunAsync(
        string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => 
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Run(hostNameOrAddress);
        }, cancellationToken);
    }

    /// <summary>
    /// Executes ping operations for multiple hosts in parallel.
    /// </summary>
    /// <param name="hostNameOrAddresses">An array of hostnames or IP addresses to ping.</param>
    /// <returns>A task that represents the asynchronous operation, containing a <see cref="PingResult"/> 
    /// with aggregated exit codes and combined output.</returns>
    public async Task<PingResult> RunAsync(params string[] hostNameOrAddresses)
    {
        return await RunAsync(hostNameOrAddresses, default);
    }

    /// <summary>
    /// Executes ping operations for multiple hosts in parallel with cancellation support.
    /// </summary>
    /// <param name="hostNameOrAddresses">A collection of hostnames or IP addresses to ping.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a <see cref="PingResult"/> 
    /// with aggregated exit codes and combined output. The output may be intermingled but will contain 
    /// all lines from all ping operations.</returns>
    /// <exception cref="TaskCanceledException">Thrown when the operation is cancelled.</exception>
    /// <remarks>
    /// This method uses thread-safe synchronization to ensure all output lines are captured correctly
    /// even when multiple ping operations complete simultaneously.
    /// </remarks>
    public async Task<PingResult> RunAsync(
        IEnumerable<string> hostNameOrAddresses, 
        CancellationToken cancellationToken = default)
    {
        var stringBuilder = new StringBuilder();
        var lockObject = new object();

        var tasks = hostNameOrAddresses.Select(hostNameOrAddress =>
            RunAsync(hostNameOrAddress, cancellationToken).ContinueWith(task =>
            {
                var result = task.Result;
                
                // Thread-safe append to stringBuilder
                lock (lockObject)
                {
                    if (result.StdOutput != null)
                    {
                        stringBuilder.Append(result.StdOutput);
                    }
                }
                
                return result.ExitCode;
            }, cancellationToken)).ToList();

        var exitCodes = await Task.WhenAll(tasks);
        int totalExitCode = exitCodes.Sum();
        
        return new PingResult(totalExitCode, stringBuilder.ToString());
    }

    /// <summary>
    /// Executes a long-running ping operation using Task.Factory.StartNew.
    /// </summary>
    /// <param name="hostNameOrAddress">The hostname or IP address to ping.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a <see cref="PingResult"/>.</returns>
    /// <exception cref="TaskCanceledException">Thrown when the operation is cancelled.</exception>
    /// <remarks>
    /// This method uses TaskCreationOptions.LongRunning to indicate that the task will run for 
    /// an extended period and should be scheduled accordingly.
    /// </remarks>
    public Task<PingResult> RunLongRunningAsync(
        string hostNameOrAddress, 
        CancellationToken cancellationToken = default)
    {
        return Task.Factory.StartNew(
            () => 
            {
                cancellationToken.ThrowIfCancellationRequested();
                return Run(hostNameOrAddress);
            },
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    /// <summary>
    /// Executes an asynchronous ping operation with progress reporting.
    /// </summary>
    /// <param name="hostNameOrAddress">The hostname or IP address to ping.</param>
    /// <param name="progress">An IProgress instance to report output lines as they occur.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task that represents the asynchronous operation, containing a <see cref="PingResult"/>.</returns>
    /// <exception cref="TaskCanceledException">Thrown when the operation is cancelled.</exception>
    /// <remarks>
    /// This method reports each line of output through the progress parameter as it is received,
    /// allowing real-time monitoring of the ping operation.
    /// </remarks>
    public async Task<PingResult> RunAsync(
        string hostNameOrAddress, 
        IProgress<string> progress, 
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        
        var startInfo = new ProcessStartInfo("ping")
        {
            Arguments = GetPingArguments(hostNameOrAddress)
        };
        
        StringBuilder? stringBuilder = null;
        
        void updateStdOutput(string? line)
        {
            (stringBuilder ??= new StringBuilder()).AppendLine(line);
            if (line != null)
            {
                progress.Report(line);
            }
        }

        var task = Task.Factory.StartNew(
            () => RunProcessInternal(startInfo, updateStdOutput, default, cancellationToken),
            cancellationToken,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
        
        var process = await task;
        return new PingResult(process.ExitCode, stringBuilder?.ToString());
    }

    /// <summary>
    /// Gets platform-specific ping arguments with a limited count.
    /// </summary>
    private static string GetPingArguments(string hostNameOrAddress)
    {
        // Limit ping count to 2 for faster tests
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return $"-n 2 {hostNameOrAddress}";
        }
        else
        {
            // macOS and Linux use -c for count
            return $"-c 2 {hostNameOrAddress}";
        }
    }

    private Process RunProcessInternal(
        ProcessStartInfo startInfo,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        var process = new Process
        {
            StartInfo = UpdateProcessStartInfo(startInfo)
        };
        return RunProcessInternal(process, progressOutput, progressError, token);
    }

    private Process RunProcessInternal(
        Process process,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        process.EnableRaisingEvents = true;
        process.OutputDataReceived += OutputHandler;
        process.ErrorDataReceived += ErrorHandler;

        try
        {
            if (!process.Start())
            {
                return process;
            }

            token.Register(obj =>
            {
                if (obj is Process p && !p.HasExited)
                {
                    try
                    {
                        p.Kill();
                    }
                    catch (Win32Exception ex)
                    {
                        throw new InvalidOperationException($"Error cancelling process{Environment.NewLine}{ex}");
                    }
                }
            }, process);

            if (process.StartInfo.RedirectStandardOutput)
            {
                process.BeginOutputReadLine();
            }
            if (process.StartInfo.RedirectStandardError)
            {
                process.BeginErrorReadLine();
            }

            if (process.HasExited)
            {
                return process;
            }
            process.WaitForExit();
        }
        catch (Exception e)
        {
            throw new InvalidOperationException($"Error running '{process.StartInfo.FileName} {process.StartInfo.Arguments}'{Environment.NewLine}{e}");
        }
        finally
        {
            if (process.StartInfo.RedirectStandardError)
            {
                process.CancelErrorRead();
            }
            if (process.StartInfo.RedirectStandardOutput)
            {
                process.CancelOutputRead();
            }
            process.OutputDataReceived -= OutputHandler;
            process.ErrorDataReceived -= ErrorHandler;

            if (!process.HasExited)
            {
                process.Kill();
            }
        }
        return process;

        void OutputHandler(object s, DataReceivedEventArgs e)
        {
            progressOutput?.Invoke(e.Data);
        }

        void ErrorHandler(object s, DataReceivedEventArgs e)
        {
            progressError?.Invoke(e.Data);
        }
    }

    private static ProcessStartInfo UpdateProcessStartInfo(ProcessStartInfo startInfo)
    {
        startInfo.CreateNoWindow = true;
        startInfo.RedirectStandardError = true;
        startInfo.RedirectStandardOutput = true;
        startInfo.UseShellExecute = false;
        startInfo.WindowStyle = ProcessWindowStyle.Hidden;

        return startInfo;
    }
}