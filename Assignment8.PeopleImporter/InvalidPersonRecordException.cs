using System.Globalization;

namespace Assignment8.ResilientImporter;


/// <summary>
/// Handles exceptions for when the person data doesn't match the record scheme
/// </summary>
public sealed class InvalidPersonRecordException : Exception
{
        public InvalidPersonRecordException()
        : this("The person record is invalid.", 0, string.Empty)
    {
    }

    public InvalidPersonRecordException(string message)
        : this(message, 0, string.Empty)
    {
    }

    public InvalidPersonRecordException(string message, Exception innerException)
        : this(message, 0, string.Empty, innerException)
    {
    }

    public InvalidPersonRecordException(string message, int lineNumber, string rawLine)
        : base(message)
    {
        LineNumber = lineNumber;
        RawLine = rawLine;
    }

    public InvalidPersonRecordException(string message, int lineNumber, string rawLine, Exception innerException)
        : base(message, innerException)
    {
        ArgumentNullException.ThrowIfNull(innerException, nameof(innerException));
        LineNumber = lineNumber;
        RawLine = rawLine;
    }

    public int LineNumber { get; }

    public string RawLine { get; }

    public override string ToString()
        => string.Create(
            CultureInfo.InvariantCulture,
            $"{base.ToString()} LineNumber: {LineNumber}; RawLine: {RawLine}");
}
