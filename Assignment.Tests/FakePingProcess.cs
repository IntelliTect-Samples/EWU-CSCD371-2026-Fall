using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;


namespace Assignment.Tests;

internal sealed class FakePingProcess : PingProcess
{
    readonly string _pingOutputTemplate = @"
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
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        Task.Delay(500, token).Wait(token);

        string host = startInfo.Arguments;

        if (host == "badaddress")
        {
            string msg =
                "Ping request could not find host badaddress. Please check the name and try again.";

            progressOutput?.Invoke(msg);
            progressOutput?.Invoke(null);
            progressError?.Invoke(null);

            return 1;
        }

        foreach (string line in GetLinesForHost(host))
        {
            progressOutput?.Invoke(line);
        }

        progressOutput?.Invoke(null);
        progressError?.Invoke(null);

        return 0;
    }

    private string[] GetLinesForHost(string host)
    {
        string[] lines = _pingOutputTemplate.Split(
            Environment.NewLine, StringSplitOptions.None);

        if (lines.Length > 0)
        {
            string first = lines[0];
            if (first.Contains('*'))
            {
                lines[0] = first.Replace("*", host);
            }
        }

        return lines;
    }
}