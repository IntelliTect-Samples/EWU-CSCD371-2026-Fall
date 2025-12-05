using Assignment;
using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment.Tests;

internal sealed class MockPingProcess : PingProcess
{
    private readonly string _pingTemplate = @"
Pinging * with 32 bytes of data:
Reply from ::1: time<1ms
Reply from ::1: time<1ms
Reply from ::1: time<1ms
Reply from ::1: time<1ms

Ping statistics for ::1:
    Packets: Sent = 4, Received = 4, Lost = 0 (0% loss),
Approximate round trip times in milli-seconds:
    Minimum = 0ms, Maximum = 0ms, Average = 0ms".Trim();

    protected override int RunProcessInternal(
        ProcessStartInfo startInfo,
        Action<string?>? stdoutCallback,
        Action<string?>? stderrCallback,
        CancellationToken cancellationToken)
    {
        // simulate network delay
        Task.Delay(400, cancellationToken).Wait(cancellationToken);

        string host = startInfo.Arguments?.Split(' ')[0] ?? "localhost";

        // simulate a host not found
        if (host.Equals("badaddress", StringComparison.OrdinalIgnoreCase))
        {
            string message = $"Ping request could not find host {host}. Please check the name and try again.";
            stdoutCallback?.Invoke(message);
            stdoutCallback?.Invoke(null);
            stderrCallback?.Invoke(null);
            return 1;
        }

        // output fake ping results
        foreach (var line in GenerateLines(host))
        {
            stdoutCallback?.Invoke(line);
        }

        stdoutCallback?.Invoke(null);
        stderrCallback?.Invoke(null);

        return 0;
    }

    private string[] GenerateLines(string host)
    {
        string[] lines = _pingTemplate.Split(Environment.NewLine, StringSplitOptions.None);
        if (lines.Length > 0 && lines[0].Contains('*'))
        {
            lines[0] = lines[0].Replace("*", host);
        }
        return lines;
    }
}
