public class InvalidPersonRecordException : Exception
{
    public int LineNumber { get; }
    public string RawLine { get; }
    public InvalidPersonRecordException()
    {
        RawLine = string.Empty;
    }
    public InvalidPersonRecordException(string message) : base(message)
    {
        RawLine = string.Empty;
    }
    public InvalidPersonRecordException(string message, Exception inner) : base(message, inner)
    {
        RawLine = string.Empty;
    }
    public InvalidPersonRecordException(string message, int lineNumber,
    string rawLine, Exception inner) : base(message, inner)
    {
        LineNumber = lineNumber;
        RawLine = rawLine;
    }
}
