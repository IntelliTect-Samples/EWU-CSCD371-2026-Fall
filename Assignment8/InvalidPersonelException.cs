namespace Assignment8;

public sealed class InvalidPersonException : Exception
{
    public InvalidPersonException() { }

    public InvalidPersonException(string message) : base(message) { }

    public InvalidPersonException(string message, Exception? innerException)
        : base(message, innerException) { }

    public InvalidPersonException(
        string message, int lineNumber, string rawLine, Exception? innerException = null)
        : base(message, innerException)
    {
        LineNumber = lineNumber;
        RawLine = rawLine;
    }

    public int LineNumber { get; }
    public string RawLine { get; } = string.Empty;
}