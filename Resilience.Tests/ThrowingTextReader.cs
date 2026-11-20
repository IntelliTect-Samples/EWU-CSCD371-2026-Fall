namespace Resilience.Tests;

public sealed class ThrowingTextReader : TextReader
{
    internal IOException _exception = new("Simulated I/O failure.");
    public override string? ReadLine()
    {
        throw _exception;
    }
}
