using System;
using System.Globalization;
using System.Runtime.CompilerServices;
[assembly: InternalsVisibleTo("PeopleImporter.Tests")]

namespace PeopleImporter;

/// <summary>
/// Parses individual CSV records into person objects.
/// </summary>
internal static class PersonRecordParser
{
    private const int ExpectedFieldCount = 8;

    /// <summary>
    /// Parses and validates a single person record.
    /// </summary>
    /// <param name="rawLine">The original CSV record.</param>
    /// <param name="lineNumber">The one-based physical line number.</param>
    /// <returns>The parsed person.</returns>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="rawLine"/> is null.
    /// </exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="lineNumber"/> is less than or equal to zero.
    /// </exception>
    /// <exception cref="InvalidPersonRecordException">
    /// The record is invalid or cannot be parsed.
    /// </exception>
    public static Person Parse(string rawLine, int lineNumber)
    {
        ArgumentNullException.ThrowIfNull(rawLine);

        if (lineNumber <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lineNumber),
                lineNumber,
                "Line number must be greater than zero.");
        }

        string[] fields = rawLine.Split(',');

        if (fields.Length != ExpectedFieldCount)
        {
            throw new InvalidPersonRecordException(
                $"Expected {ExpectedFieldCount} fields but found {fields.Length}.",
                lineNumber,
                rawLine);
        }

        for (int i = 0; i < fields.Length; i++)
        {
            fields[i] = fields[i].Trim();
        }

        ValidateRequiredField(
            fields[1],
            nameof(IPerson.FirstName),
            lineNumber,
            rawLine);

        ValidateRequiredField(
            fields[2],
            nameof(IPerson.LastName),
            lineNumber,
            rawLine);

        ValidateRequiredField(
            fields[3],
            nameof(IPerson.EmailAddress),
            lineNumber,
            rawLine);

        ValidateRequiredField(
            fields[4],
            nameof(IAddress.StreetAddress),
            lineNumber,
            rawLine);

        ValidateRequiredField(
            fields[5],
            nameof(IAddress.City),
            lineNumber,
            rawLine);

        ValidateRequiredField(
            fields[6],
            nameof(IAddress.State),
            lineNumber,
            rawLine);

        ValidateRequiredField(
            fields[7],
            nameof(IAddress.Zip),
            lineNumber,
            rawLine);

        try
        {
            int id = int.Parse(fields[0], CultureInfo.InvariantCulture);

            if (id <= 0)
            {
                throw new FormatException("Id must be greater than zero.");
            }

            Address address = new(
                fields[4],
                fields[5],
                fields[6],
                fields[7]);

            return new Person(
                id,
                fields[1],
                fields[2],
                address,
                fields[3]);
        }
        catch (InvalidPersonRecordException)
        {
            throw;
        }
        catch (Exception ex) when (
            ex is FormatException ||
            ex is OverflowException ||
            ex is ArgumentOutOfRangeException)
        {
            throw new InvalidPersonRecordException(
                "The record contains invalid data.",
                lineNumber,
                rawLine,
                ex);
        }
    }

    private static void ValidateRequiredField(
        string value,
        string fieldName,
        int lineNumber,
        string rawLine)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            throw new InvalidPersonRecordException(
                $"{fieldName} is required.",
                lineNumber,
                rawLine);
        }
    }
}