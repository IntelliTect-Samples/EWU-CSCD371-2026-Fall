namespace PeopleImport;

public sealed class InvalidPersonRecordException : Exception
{
    public int LineNumber { get; }
    public string? RawLine { get; }

    public InvalidPersonRecordException() : base("The person record is invalid.") { }

    public InvalidPersonRecordException(string? message) : base(message) { }

    public InvalidPersonRecordException(string? message, Exception? innerException)
        : base(message, innerException) { }

    public InvalidPersonRecordException(int lineNumber, string rawLine, string message, Exception? innerException = null)
        : base(message, innerException)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(lineNumber);
        ArgumentNullException.ThrowIfNull(rawLine);
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        LineNumber = lineNumber;
        RawLine = rawLine;
    }
}
