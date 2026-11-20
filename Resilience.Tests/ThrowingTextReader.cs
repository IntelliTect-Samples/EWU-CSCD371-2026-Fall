namespace Resilience.Tests;

public sealed class ThrowingTextReader : TextReader
{
    internal IOException Exception = new("Simulated I/O failure.");
    public override string? ReadLine()
    {
        throw Exception;
    }
}
