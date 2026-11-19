namespace Assignment;

public class InvalidPersonRecordException : Exception
{
    public int LineNumber { get; }
    public string? RawLine { get; }

    public InvalidPersonRecordException() { }
    public InvalidPersonRecordException(string message) : base(message) { }
    public InvalidPersonRecordException(string message, Exception inner) : base(message, inner) { }

    public InvalidPersonRecordException(int lineNumber, string rawLine, Exception inner)
        : base($"Invalid person record at line {lineNumber} ({inner?.Message}): {rawLine}", inner)
    {
        LineNumber = lineNumber;
        RawLine = rawLine;
    }
}