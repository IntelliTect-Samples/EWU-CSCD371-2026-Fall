using System;

namespace PeopleImporter;

/// <summary>
/// Parses individual CSV records into person objects.
/// </summary>
internal static class PersonRecordParser
{
    /// <summary>
    /// Parses and validates a single person record.
    /// </summary>
    /// <param name="rawLine">The original CSV record.</param>
    /// <param name="lineNumber">The one-based physical line number.</param>
    /// <returns>The parsed person.</returns>
    /// <exception cref="InvalidPersonRecordException">
    /// The record is invalid or cannot be parsed.
    /// </exception>
    public static Person Parse(string rawLine, int lineNumber)
    {
        // TODO: Partner implements CSV parsing and validation.
        throw new NotImplementedException(
            "Person record parsing has not been implemented.");
    }
}