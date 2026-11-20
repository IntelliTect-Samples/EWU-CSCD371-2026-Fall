namespace Resilience.Tests;

public sealed class ThrowingTextReader : TextReader
{
    public IOException Exception = new("Simulated I/O failure.");
    public override string? ReadLine()
    {
        throw Exception;
    }
}
