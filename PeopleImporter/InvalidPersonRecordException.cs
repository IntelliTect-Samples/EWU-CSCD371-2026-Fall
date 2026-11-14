using System;

namespace PeopleImporter;

/// <summary>
/// Represents an error encountered while parsing or validating a person CSV record.
/// </summary>
/// <remarks>
/// Line numbers are one-based and include the CSV header.
/// </remarks>
public sealed class InvalidPersonRecordException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="InvalidPersonRecordException"/> class.
    /// </summary>
    public InvalidPersonRecordException()
        : base("An invalid person record was encountered.")
    {
    }

    /// <summary>
    /// Initializes a new instance with a specified error message.
    /// </summary>
    /// <param name="message">The message describing the error.</param>
    public InvalidPersonRecordException(string? message)
        : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance with a specified error message and inner exception.
    /// </summary>
    /// <param name="message">The message describing the error.</param>
    /// <param name="innerException">The exception that caused this error.</param>
    public InvalidPersonRecordException(
        string? message,
        Exception? innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    /// Initializes a new instance with information about an invalid CSV record.
    /// </summary>
    /// <param name="message">The message describing the error.</param>
    /// <param name="lineNumber">The one-based line number of the invalid record.</param>
    /// <param name="rawLine">The original CSV line.</param>
    public InvalidPersonRecordException(
        string message,
        int lineNumber,
        string rawLine)
        : this(message, lineNumber, rawLine, null)
    {
    }

    /// <summary>
    /// Initializes a new instance with record information and an inner exception.
    /// </summary>
    /// <param name="message">The message describing the error.</param>
    /// <param name="lineNumber">The one-based line number of the invalid record.</param>
    /// <param name="rawLine">The original CSV line.</param>
    /// <param name="innerException">The exception that caused this error.</param>
    public InvalidPersonRecordException(
        string message,
        int lineNumber,
        string rawLine,
        Exception? innerException)
        : base(message, innerException)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(message);

        if (lineNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lineNumber),
                lineNumber,
                "Line number must be greater than zero.");
        }

        ArgumentNullException.ThrowIfNull(rawLine);

        LineNumber = lineNumber;
        RawLine = rawLine;
    }

    /// <summary>
    /// Gets the one-based CSV line number containing the invalid record.
    /// </summary>
    public int? LineNumber { get; }

    /// <summary>
    /// Gets the original CSV line containing the invalid record.
    /// </summary>
    public string? RawLine { get; }
}