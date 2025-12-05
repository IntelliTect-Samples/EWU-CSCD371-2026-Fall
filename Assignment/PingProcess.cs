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

public record struct PingResult(int ExitCode, string? StdOutput);

/// <summary>
/// This class basically runs "ping" in your cmd (or something similar)
/// </summary>
public class PingProcess
{
    private ProcessStartInfo StartInfo { get; } = new("ping");

    public virtual PingResult Run(string hostNameOrAddress)
    {
        string termination = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? " -n 4" : " -c 4";

        StartInfo.Arguments = hostNameOrAddress + termination;
        StringBuilder? stringBuilder = null;
        void updateStdOutput(string? line) => (stringBuilder??=new StringBuilder()).AppendLine(line);
        Process process = RunProcessInternal(StartInfo, updateStdOutput, default, default);
        return new PingResult(process.ExitCode, stringBuilder?.ToString());
    }

    public Task<PingResult> RunTaskAsync(string hostNameOrAddress)
    {
        return Task.Run(() => { return Run(hostNameOrAddress); });
    }

    async public Task<PingResult> RunAsync(string hostNameOrAddress, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        Task<PingResult> task = Task.Run(() => { return Run(hostNameOrAddress); }, cancellationToken);
        return await task;
    }

    async public Task<PingResult> RunAsync(CancellationToken token = default, params string[] hostNameOrAddresses)
    {
        StringBuilder? stringBuilder = new();
        object _Sync = new();

        Task<int>[] all = hostNameOrAddresses
            .AsParallel()
            .Select(async item =>
            {
                PingResult output = await RunTaskAsync(item).WaitAsync(token);
                lock(_Sync)
                {
                    stringBuilder.Append(output.StdOutput);
                }
                return output.ExitCode;
            }).ToArray();
        await Task.WhenAll(all);
        int total = all.Aggregate(0, (total, item) => total + item.Result);
        return new PingResult(total, stringBuilder.ToString());
    }

    public Task<int> RunLongRunningAsync(ProcessStartInfo startInfo, Action<string?>? progressOutput,
        Action<string?>? progressError, CancellationToken token)
    {
        return Task.Factory.StartNew(() =>{
            return RunProcessInternal(startInfo, progressOutput, progressError, token).ExitCode; 
        }, token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
    }

    private Process RunProcessInternal(ProcessStartInfo startInfo, Action<string?>? progressOutput, Action<string?>? progressError, CancellationToken token)
    {
        var process = new Process
        {
            StartInfo = UpdateProcessStartInfo(startInfo)
        };
        return RunProcessInternal(process, progressOutput, progressError, token);
    }

    private Process RunProcessInternal(Process process, Action<string?>? progressOutput, Action<string?>? progressError, CancellationToken token)
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