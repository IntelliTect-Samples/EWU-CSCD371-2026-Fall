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
    public PingResult Run(string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        ProcessStartInfo info = new("ping")
        {
            Arguments = hostNameOrAddress
        };

        StringBuilder? stringBuilder = null;
        void updateStdOutput(string? line) =>
            (stringBuilder ??= new StringBuilder()).AppendLine(line);
        int exitCode = RunProcessInternal(info, updateStdOutput, default, cancellationToken);
        return new PingResult(exitCode, stringBuilder?.ToString());
    }

    public Task<PingResult> RunTaskAsync(string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        return Task.Run(() => Run(hostNameOrAddress, cancellationToken));
    }

    public async Task<PingResult> RunAsync(string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Task<PingResult> task = RunTaskAsync(hostNameOrAddress, cancellationToken);
        PingResult result = await task;
        return result;
    }

    public async Task<PingResult> RunAsync(string hostNameOrAddress, IProgress<string?> progress, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(progress);

        StringBuilder stringBuilder = new();

        var progressOutput = WrapProgress(progress, stringBuilder);
        var progressError = WrapProgress(progress, stringBuilder);

        ProcessStartInfo startInfo = new("ping", hostNameOrAddress);

        Task<int> longRunningTask =
            RunLongRunningAsync(startInfo, progressOutput, progressError, cancellationToken);

        int exitCode = await longRunningTask.WaitAsync(cancellationToken);

        string? combinedOutput = stringBuilder.Length > 0
            ? stringBuilder.ToString()
            : null;

        return new PingResult(exitCode, combinedOutput);
    }

    public async Task<PingResult> RunAsync(IEnumerable<string> hostNameOrAddresses, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(hostNameOrAddresses);

        StringBuilder? stringBuilder = null;
        object syncRoot = new();

        Task<int>[] tasks = hostNameOrAddresses.Select(async host =>
        {
            Task<PingResult> task = RunTaskAsync(host, cancellationToken);
            PingResult result = await task.WaitAsync(cancellationToken);

            if (!string.IsNullOrEmpty(result.StdOutput))
            {
                lock (syncRoot)
                {
                    (stringBuilder ??= new StringBuilder())
                        .Append(result.StdOutput);
                }
            }

            return result.ExitCode;
        }).ToArray();

        await Task.WhenAll(tasks);
        int total = tasks.Aggregate(0, (total, item) => total + item.Result);
        string? combinedOutput = stringBuilder?.Length > 0
            ? stringBuilder.ToString()
            : null;
        return new PingResult(total, combinedOutput);
    }

    public Task<int> RunLongRunningAsync(ProcessStartInfo startInfo, Action<string?>? progressOutput,
        Action<string?>? progressError, CancellationToken token)
    {
        return Task.Factory.StartNew(
            () =>
            {
                return RunProcessInternal(startInfo, progressOutput, progressError, token);
            },
            token,
            TaskCreationOptions.LongRunning,
            TaskScheduler.Default);
    }

    protected virtual int RunProcessInternal(
        ProcessStartInfo startInfo,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        using var process = new Process
        {
            StartInfo = UpdateProcessStartInfo(startInfo)
        };

        return RunProcessInternal(process, progressOutput, progressError, token);
    }

    private int RunProcessInternal(
        Process process,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        using ManualResetEventSlim? outputDone =
            process.StartInfo.RedirectStandardOutput ? new(initialState: false) : null;
        using ManualResetEventSlim? errorDone =
            process.StartInfo.RedirectStandardError ? new(initialState: false) : null;

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
                        p.Kill(entireProcessTree: true);
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

            if (!process.HasExited)
            {
                process.WaitForExit();
            }

            outputDone?.Wait(token);
            errorDone?.Wait(token);
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
            if (e.Data is null)
            {
                outputDone?.Set();
            }
        }

        void ErrorHandler(object s, DataReceivedEventArgs e)
        {
            progressError?.Invoke(e.Data);
            if (e.Data is null)
            {
                errorDone?.Set();
            }
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

    private static Action<string?> WrapProgress(
    IProgress<string?> progress,
    StringBuilder stringBuilder)
    {
        return line =>
        {
            progress.Report(line);

            if (line is null)
                return;

            stringBuilder.AppendLine(line);
        };
    }
}