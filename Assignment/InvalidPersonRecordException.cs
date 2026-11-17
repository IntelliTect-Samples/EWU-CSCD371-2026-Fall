namespace Assignment;

public sealed class InvalidPersonRecordException : Exception
{
    public InvalidPersonRecordException() : base("Invalid person record.")
    {
        RawLine = string.Empty;
    }

    public InvalidPersonRecordException(string? message) : base(message)
    {
        RawLine = string.Empty;
    }

    public InvalidPersonRecordException(string? message, Exception? innerException)
        : base(message, innerException)
    {
        RawLine = string.Empty;
    }

    public InvalidPersonRecordException(string? message, int lineNumber, string rawLine)
        : this(message, lineNumber, rawLine, null)
    {
    }

    public InvalidPersonRecordException(
        string? message,
        int lineNumber,
        string rawLine,
        Exception? innerException)
        : base(message, innerException)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(lineNumber);
        ArgumentNullException.ThrowIfNull(rawLine);

        LineNumber = lineNumber;
        RawLine = rawLine;
    }

    public int LineNumber { get; }
    public string RawLine { get; }
}
