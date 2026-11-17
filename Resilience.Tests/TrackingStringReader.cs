namespace Resilience.Tests;

public class TrackingStringReader : StringReader
{
    public bool IsDisposed { get; private set; }

    public TrackingStringReader(string s) : base(s)
    {
        IsDisposed = false;
    }

    protected override void Dispose(bool disposing)
    {
        IsDisposed = true;
        base.Dispose(disposing);
    }
}
