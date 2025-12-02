using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assignment.Tests;

public class PingProcessMock : PingProcess
{
    public override PingResult Run(string hostNameOrAddress)
    {
        if(hostNameOrAddress == "badaddress")
        {
            return new PingResult(1, "Ping request could not find host badaddress. Please check the name and try again.".Trim());
        }

        string pingOutput = @"
Pinging * with 32 bytes of data:
Reply from ::1: time<*
Reply from ::1: time<*
Reply from ::1: time<*
Reply from ::1: time<*

Ping statistics for ::1:
    Packets: Sent = *, Received = *, Lost = 0 (0% loss),
Approximate round trip times in milli-seconds:
    Minimum = *, Maximum = *, Average = *".Trim();

        return new PingResult(0, pingOutput);
    }
}
