using System;
using System.Collections.Generic;
using System.Text;

namespace Resilience.Tests;
public sealed class ThrowingTextReader : TextReader
{
    public override string? ReadLine()
    {
        throw new IOException(
            "Simulated I/O failure.");
    }
}
