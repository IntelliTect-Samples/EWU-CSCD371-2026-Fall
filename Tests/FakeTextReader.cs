namespace Tests;

/// <summary>
/// A <see cref="TextReader"/> for tests. It returns the given lines one at a time, then either
/// returns <see langword="null"/> (end of input) or throws the supplied exception, simulating an
/// I/O failure partway through a file. It also counts how many times it was disposed.
/// </summary>
internal sealed class FakeTextReader : TextReader
{
    private readonly Queue<string> _lines;
    private readonly Exception? _exceptionAfterLines;

    public FakeTextReader(IEnumerable<string> lines, Exception? exceptionAfterLines = null)
    {
        ArgumentNullException.ThrowIfNull(lines);

        _lines = new Queue<string>(lines);
        _exceptionAfterLines = exceptionAfterLines;
    }

    /// <summary>Gets how many times <see cref="Dispose(bool)"/> has run.</summary>
    public int DisposeCount { get; private set; }

    public override string? ReadLine()
    {
        if (_lines.Count > 0)
        {
            return _lines.Dequeue();
        }

        if (_exceptionAfterLines is not null)
        {
            throw _exceptionAfterLines;
        }

        return null;
    }

    protected override void Dispose(bool disposing)
    {
        DisposeCount++;
        base.Dispose(disposing);
    }
}
