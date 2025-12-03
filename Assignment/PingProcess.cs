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
    private readonly object _stdOutputLock = new();

    public PingResult Run(string hostNameOrAddress)
    {
        StartInfo.Arguments = FormatPingArguments(hostNameOrAddress);
        StringBuilder? stringBuilder = null;
        void updateStdOutput(string? line) =>
            (stringBuilder??=new StringBuilder()).AppendLine(line);
        Process process = RunProcessInternal(StartInfo, updateStdOutput, default, default);
        return new PingResult( process.ExitCode, stringBuilder?.ToString());
    }

    public Task<PingResult> RunTaskAsync(string hostNameOrAddress)
    {
        return Task.Run(() => Run(hostNameOrAddress));
    }

    public async Task<PingResult> RunAsync(
        string hostNameOrAddress,
        CancellationToken cancellationToken = default)
    {
        return await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();

            StartInfo.Arguments = FormatPingArguments(hostNameOrAddress);

            StringBuilder? sb = null;
            void updateStdOutput(string? line) =>
                (sb ??= new StringBuilder()).AppendLine(line);

            var process = RunProcessInternal(
                StartInfo,
                updateStdOutput,
                progressError: null,
                token: cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            return new PingResult(process.ExitCode, sb?.ToString());
        }, cancellationToken).ConfigureAwait(false);
    }

    public async Task<PingResult> RunAsync(
        IEnumerable<string> hostNameOrAddresses,
        CancellationToken cancellationToken = default)
    {
        StringBuilder sharedStringBuilder = new();

        void UpdateSharedStdOutput(string? line)
        {
            if (line is null) return;

            lock (_stdOutputLock)
            {
                sharedStringBuilder.AppendLine(line);
            }
        }

        var pingTasks = hostNameOrAddresses.Select(host =>
            Task.Run(() =>
            {
                cancellationToken.ThrowIfCancellationRequested();

                var startInfo = new ProcessStartInfo("ping")
                {
                    Arguments = FormatPingArguments(host)
                };

                var process = RunProcessInternal(startInfo, UpdateSharedStdOutput, default, cancellationToken);
                return process.ExitCode;
            }, cancellationToken)
        ).ToList();

        int[] exitCodes = await Task.WhenAll(pingTasks).ConfigureAwait(false);

        int totalExitCode = exitCodes.Sum();

        return new PingResult(totalExitCode, sharedStringBuilder.ToString());
    }

    public Task<PingResult> RunAsync(params string[] hostNameOrAddresses) =>
        RunAsync((IEnumerable<string>)hostNameOrAddresses);

    public Task<int> RunLongRunningAsync(
        ProcessStartInfo startInfo,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        return Task.Factory.StartNew(() =>
        {
            var process = RunProcessInternal(startInfo, progressOutput, progressError, token);
            return process.ExitCode;
        }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    public async Task<PingResult> RunLongRunningAsync(
        string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        StringBuilder sb = new();
        void updateStdOutput(string? line) => sb.AppendLine(line);

        var startInfo = new ProcessStartInfo("ping")
        {
            Arguments = FormatPingArguments(hostNameOrAddress)
        };

        int exitCode = await RunLongRunningAsync(
            startInfo,
            updateStdOutput,
            progressError: null,
            token: cancellationToken).ConfigureAwait(false);

        return new PingResult(exitCode, sb.ToString());
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

    private static string FormatPingArguments(string host)
{
    if (OperatingSystem.IsWindows())
    {
        return $"-n 4 {host}";
    }
    else
    {
        return $"-c 4 {host}";
    }
}
}