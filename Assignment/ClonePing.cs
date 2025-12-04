using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment;

public class ClonePing : PingProcess
{
    private readonly string _Template = @"
Pinging {HOST} with 32 bytes of data:
Reply from ::1: time<1ms
Reply from ::1: time<1ms
Reply from ::1: time<1ms
Reply from ::1: time<1ms

Ping statistics for ::1:
    Packets: Sent = 4, Received = 4, Lost = 0 (0% loss),
Approximate round trip times in milli-seconds:
    Minimum = 0ms, Maximum = 1ms, Average = 0ms".Trim();

    private static readonly string[] NewLineSeparators = ["\r\n", "\n"];
    private static readonly char[] HostSeparators = [' ', '\t'];
    protected override int RunProcessInternal(ProcessStartInfo startInfo, Action<string?>? progressOutput, Action<string?>? progressError, CancellationToken token)
    {
        token.ThrowIfCancellationRequested();

        string args = startInfo.Arguments ?? string.Empty;
        string host = ExtractHost(args); 

        if (host.Equals("badaddress", StringComparison.OrdinalIgnoreCase))
        {
            string message = "Ping request could not find host badaddress. Please check the name and try again.";
            progressOutput?.Invoke(message);
            progressOutput?.Invoke(null);
            progressError?.Invoke(null);

            return 1;
        }

        string text = _Template.Replace("{HOST}", host);

        foreach (string line in text.Split(NewLineSeparators, StringSplitOptions.None))
        {
            progressOutput?.Invoke(line);
        }


        progressOutput?.Invoke(null);
        progressError?.Invoke(null);

        return 0;
    }
    private static string ExtractHost(string arguments)
    {
        if (string.IsNullOrWhiteSpace(arguments))
        {
            return "localhost";
        }


        string[] parts = arguments.Split(
        HostSeparators, StringSplitOptions.RemoveEmptyEntries);

        return parts.Length == 0 ? "localhost" : parts[^1];
    }

}

