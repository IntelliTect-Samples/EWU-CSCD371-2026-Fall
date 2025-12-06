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
        StartInfo.Arguments = hostNameOrAddress;
        StringBuilder? stringBuilder = null;
        void updateStdOutput(string? line) =>
            (stringBuilder ??= new StringBuilder()).AppendLine(line);
        var processExitCode = RunProcessInternal(StartInfo, updateStdOutput, default, default);
        return new PingResult(processExitCode, stringBuilder?.ToString());
    }

    public Task<PingResult> RunTaskAsync(string hostNameOrAddress)
    {
        return Task.Run(() => Run(hostNameOrAddress));
    }

    async public Task<PingResult> RunAsync(
        string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        PingResult result = await Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            return Run(hostNameOrAddress);
        }, cancellationToken);

        return result;
    }

    async public Task<PingResult> RunAsync(IEnumerable<string> hostNameOrAddresses, CancellationToken cancellationToken = default)
    {
        List<Task<(int exitCode, List<string> collected)>> tasks = hostNameOrAddresses.Select(address => Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            List<string> collected = new();

            void capture(string? line)
            {
                if (line != null) collected.Add(line);
            }

            ProcessStartInfo psi = new("ping") { Arguments = address };
            int exitCode = RunProcessInternal(psi, capture, null, cancellationToken);

            return (exitCode, collected);
        }, cancellationToken)).ToList();

        (int exitCode, List<string> collected)[] results = await Task.WhenAll(tasks);

        StringBuilder builder = new();
        int totalExitCode = 0;

        foreach ((int exitCode, List<string> lines) in results)
        {
            totalExitCode += exitCode;
            foreach (string line in lines)
                builder.AppendLine(line);
        }

        return new PingResult(totalExitCode, builder.ToString().TrimEnd());
    }

    async public Task<PingResult> RunLongRunningAsync(
        string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        StringBuilder? stringBuilder = null;
        void capture(string? line)
        {
            if (line is null) return;
            (stringBuilder ??= new StringBuilder()).AppendLine(line);
        }

        var startInfo = new ProcessStartInfo("ping") { Arguments = hostNameOrAddress };
        var task = Task.Factory.StartNew(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            var processExitCode = RunProcessInternal(startInfo, capture, null, cancellationToken);
            return new PingResult(processExitCode, stringBuilder?.ToString());
        }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Current);

        return await task;
    }

    public Task<int> RunLongRunningAsync(
        ProcessStartInfo startInfo,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        return Task.Factory.StartNew(() =>
        {
            token.ThrowIfCancellationRequested();
            var processExitCode = RunProcessInternal(startInfo, progressOutput, progressError, token);
            return processExitCode;
        }, token, TaskCreationOptions.LongRunning, TaskScheduler.Current);
    }

    public Task<PingResult> RunAsync(
        string hostNameOrAddress,
        IProgress<string?> progress,
        CancellationToken cancellationToken = default)
    {
        return Task.Run(() =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            StringBuilder? sb = null;
            void capture(string? line)
            {
                progress?.Report(line);
                if (line is null) return;
                (sb ??= new StringBuilder()).AppendLine(line);
            }
            var info = new ProcessStartInfo("ping") { Arguments = hostNameOrAddress };
            var processExitCode = RunProcessInternal(info, capture, null, cancellationToken);
            return new PingResult(processExitCode, sb?.ToString());
        }, cancellationToken);
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

    protected virtual int RunProcessInternal(
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

            process.WaitForExit();
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