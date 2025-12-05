using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Assignment;

public record struct PingResult(int ExitCode, string? StdOutput);

public class PingProcess
{

    public PingResult Run(string hostNameOrAddress, CancellationToken cancellationToken = default )
    {
        string args = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? " -n 4" : " -c 4";

        ProcessStartInfo info = new("ping")
        {
            Arguments = hostNameOrAddress + args
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

    async public Task<PingResult> RunAsync(string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        Task<PingResult> task = RunTaskAsync(hostNameOrAddress, cancellationToken);
        PingResult result = await task.WaitAsync(cancellationToken);
        cancellationToken.ThrowIfCancellationRequested();
        return result;
    }

    async public Task<PingResult> RunAsync(IEnumerable<string> hostNameOrAddresses, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(hostNameOrAddresses);

        StringBuilder stringBuilder = new();
        object lockObject = new();

        Task<int>[] tasks = hostNameOrAddresses.Select(async host =>
        {
            cancellationToken.ThrowIfCancellationRequested();
            PingResult result = await RunAsync(host, cancellationToken);

            string output = result.StdOutput?.Trim() ?? string.Empty;
            if (!string.IsNullOrEmpty(output))
            {
                lock (lockObject)
                {
                    stringBuilder.AppendLine(output);
                }
            }

            return result.ExitCode;
        }).ToArray();

        int[] results = await Task.WhenAll(tasks);
        int totalExitCode = results.Any(code => code != 0) ? 1 : 0;
        string? combinedOutput = stringBuilder.Length > 0 ? stringBuilder.ToString() : null;

        return new PingResult(totalExitCode, combinedOutput);
    }

    public Task<int> RunLongRunningAsync(
                        ProcessStartInfo startInfo,
                        Action<string?>? progressOutput,
                        Action<string?>? progressError,
                        CancellationToken token)
    {
        ArgumentNullException.ThrowIfNull(startInfo);

        return Task.Factory.StartNew(() =>
        { 
            return RunProcessInternal(startInfo, progressOutput, progressError, token);

        },
        token,
        TaskCreationOptions.LongRunning,
        TaskScheduler.Current);
    }


    // extra credit
    public async Task<PingResult> RunAsync(
        string hostNameOrAddresses, 
        IProgress<string?> progress, 
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(progress);
        ArgumentNullException.ThrowIfNull(hostNameOrAddresses);

        StringBuilder outputBuilder = new();
        void captureLine(string? line)
        {
            if (line is null)
                return;

            lock (outputBuilder)
            {
                outputBuilder.AppendLine(line);
            }

            progress.Report(line);
        }

        return await Task.Run(() =>
        {
            ProcessStartInfo startInfo = new ProcessStartInfo("ping", hostNameOrAddresses);
            int exitCode = RunProcessInternal(startInfo, captureLine, null, cancellationToken);

            if (exitCode != 0) exitCode = 1;

            return new PingResult(exitCode, outputBuilder.ToString());
        });
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

    private int RunProcessInternal(
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
