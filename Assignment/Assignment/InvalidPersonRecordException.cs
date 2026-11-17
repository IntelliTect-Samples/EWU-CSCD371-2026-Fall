using System;

namespace Assignment;

public class InvalidPersonRecordException : Exception
{
    public int LineNumber { get; }
    public string RawLine { get; } = string.Empty;

    public InvalidPersonRecordException()
        : base("Invalid person record.")
    {
    }

    public InvalidPersonRecordException(string message)
        : base(message)
    {
    }

    public InvalidPersonRecordException(
        string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public InvalidPersonRecordException(
        int lineNumber, string rawLine, string reason,
        Exception? innerException = null)
        : base($"Invalid person record on line {lineNumber}: {reason}",
            innerException)
    {
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(lineNumber);
        ArgumentNullException.ThrowIfNull(rawLine);
        ArgumentException.ThrowIfNullOrWhiteSpace(reason);

        LineNumber = lineNumber;
        RawLine = rawLine;
    }
}