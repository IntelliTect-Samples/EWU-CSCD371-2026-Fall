using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace Assignment.Tests;

internal sealed class PingProcessMock : PingProcess
{
    private static string CreatePingBlock(string host) => $@"
Pinging {host} with 32 bytes of data:
Reply from ::1: time<1ms
Reply from ::1: time<1ms
Reply from ::1: time<1ms
Reply from ::1: time<1ms

Ping statistics for ::1:
    Packets: Sent = 4, Received = 4, Lost = 0 (0% loss),
Approximate round trip times in milli-seconds:
    Minimum = 1ms, Maximum = 1ms, Average = 1ms".Trim();

    protected override int RunProcessInternal(
        ProcessStartInfo startInfo,
        Action<string?>? progressOutput,
        Action<string?>? progressError,
        CancellationToken token)
    {
        string args = startInfo?.Arguments ?? string.Empty;
        string host = args.Trim();

        token.ThrowIfCancellationRequested();

        if (host.Contains("badaddress", StringComparison.OrdinalIgnoreCase))
        {
            string msg =
                "Ping request could not find host badaddress. Please check the name and try again.";
            progressOutput?.Invoke(msg);
            progressOutput?.Invoke(null);
            progressError?.Invoke(null);
            return 1;
        }

        // Add small delay to allow cancellation testing
        Task.Delay(50, token).Wait(token);

        string block = CreatePingBlock(host);
        foreach (var line in block.Split(Environment.NewLine))
        {
            token.ThrowIfCancellationRequested();
            progressOutput?.Invoke(line);
        }

        // Send null to signal completion (matches real process behavior)
        progressOutput?.Invoke(null);
        progressError?.Invoke(null);

        return 0;
    }
}
