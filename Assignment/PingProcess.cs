using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment;

public record struct PingResult(int ExitCode, string? StdOutput);

public class PingProcess
{
    private ProcessStartInfo StartInfo { get; } = new("ping");

    public PingResult Run(string hostNameOrAddress)
    {
        StartInfo.Arguments = BuildPingArguments(hostNameOrAddress);
        StringBuilder? stringBuilder = null;

        void updateStdOutput(string? line) =>
            (stringBuilder??=new StringBuilder()).AppendLine(line);
        
        int exit = RunProcessInternal(StartInfo, updateStdOutput, updateStdOutput, default);
        return new PingResult( exit, stringBuilder?.ToString());
    }

    public Task<PingResult> RunTaskAsync(string hostNameOrAddress)
    {
        return Task.Run(() =>
        { 
            StartInfo.Arguments = BuildPingArguments(hostNameOrAddress);
            StringBuilder? stringBuilder = null;
            void updateStdOutput(string? line) =>
                (stringBuilder ??= new StringBuilder()).AppendLine(line);

           int exit = RunProcessInternal(StartInfo, updateStdOutput, updateStdOutput, default);
            return new PingResult(exit, stringBuilder?.ToString());
        });
    }

    async public Task<PingResult> RunAsync(
        string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        StartInfo.Arguments = BuildPingArguments(hostNameOrAddress);
        StringBuilder? stringBuilder = null;

        void updateStdOutput(string? line) =>
            (stringBuilder ??= new StringBuilder()).AppendLine(line);

        int exit = await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return RunProcessInternal(StartInfo, updateStdOutput, default, cancellationToken);

        }, cancellationToken);

        return new PingResult(exit, stringBuilder?.ToString());
    }

    public Task<PingResult> RunAsync(string hostNameOrAddresses, IProgress<string?> progress, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(progress);

        return Task.Run(() =>
        {
            var startInfo = new ProcessStartInfo("ping")
            {
                Arguments = BuildPingArguments(hostNameOrAddresses)
            };

            StringBuilder? stringBuilder = null;

            void updateStdOutput(string? line)
            {

                if (line is not null)
                {
                    (stringBuilder ??= new StringBuilder()).AppendLine(line);
                }

                progress.Report(line);

            }

            cancellationToken.ThrowIfCancellationRequested();
            
            int process = RunProcessInternal(startInfo, updateStdOutput, default, cancellationToken);
            string? stdOutput = stringBuilder?.ToString(); 
            return Task.FromResult (new PingResult(process, stdOutput));
        }, cancellationToken);
        
    }

    /// <summary>
    /// Convenience wrapper for params[] that delegates to the IEnumerable<T> overload.
    /// </summary>
    public Task<PingResult> RunAsync(params string[] hostNameOrAddresses) =>
        RunAsync((IEnumerable<string>)hostNameOrAddresses, default);

    /// <summary>
    /// Multi-host async ping implementation.
    /// </summary>
    public async Task<PingResult> RunAsync(
    IEnumerable<string> hostNameOrAddresses,
    CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(hostNameOrAddresses);

        StringBuilder stringBuilder = new();
        object syncLock = new();

        Task<int>[] tasks = hostNameOrAddresses
            .Select(host => Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var startInfo = new ProcessStartInfo("ping")
                {
                    Arguments = BuildPingArguments(host)
                };

                void updateStdOutput(string? line)
                {
                    if (line is null) return;

                    lock (syncLock)
                    {
                        stringBuilder.AppendLine(line);
                    }
                }

                int exit = RunProcessInternal(startInfo, updateStdOutput, default, cancellationToken);
                return exit;
            }, cancellationToken))
            .ToArray();

        int[] exitCodes = await Task.WhenAll(tasks);

        int totalExitCode = exitCodes.Sum();
        string combinedOutput = stringBuilder.ToString().TrimEnd('\r', '\n');
        return new PingResult(totalExitCode, combinedOutput);
    }


    /// <summary>
    /// Implemented using Task.Factory.StartNew with TaskCreationOptions.LongRunning
    /// and TaskScheduler.Current. Returns the process exit code.
    /// </summary>
    public Task<int> RunLongRunningAsync(
        ProcessStartInfo startInfo,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(startInfo);

        return Task.Factory.StartNew(() =>
        {
            int exit = RunProcessInternal(startInfo, progressOutput, progressError, token);
            return exit;
        }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }


    /// <summary>
    /// A convenience overload that pings a host and returns PingResult,
    /// but still uses Task.Factory.StartNew with LongRunning option.
    /// </summary>
    public Task<PingResult> RunLongRunningAsync(
    string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(hostNameOrAddress);

        var startInfo = new ProcessStartInfo("ping")
        {
            Arguments = BuildPingArguments(hostNameOrAddress)
        };

        StringBuilder? stringBuilder = null;
        void updateStdOutput(string? line) =>
            (stringBuilder ??= new StringBuilder()).AppendLine(line);

        return Task.Factory.StartNew(() =>
        {
            int exit = RunProcessInternal(startInfo, updateStdOutput, default, cancellationToken);
            return new PingResult(exit, stringBuilder?.ToString());
        }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

   protected virtual int RunProcessInternal(
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

    private static string BuildPingArguments(string hostNameOrAddress)
    {
        if (OperatingSystem.IsWindows())
        {
            return $"-n 4 {hostNameOrAddress}";
        }
        else
        {
            return $"-c 4 {hostNameOrAddress}";
        }
    }


    private static int RunProcessInternal(
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
                return process.ExitCode;
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
                return process.ExitCode;
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
        return process.ExitCode;

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