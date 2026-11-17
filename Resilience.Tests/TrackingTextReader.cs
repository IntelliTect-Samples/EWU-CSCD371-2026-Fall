namespace Resilience.Tests;

public class TrackingTextReader : TextReader
{
    public bool IsDisposed { get; private set; }

    TextReader _textReader;

    public TrackingTextReader(TextReader textReader)
    {
        _textReader = textReader;
        IsDisposed = false;
    }

    public override string? ReadLine()
    {
        return _textReader.ReadLine();
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _textReader.Dispose();
            IsDisposed = true;
        }

        base.Dispose(disposing);
    }
}
