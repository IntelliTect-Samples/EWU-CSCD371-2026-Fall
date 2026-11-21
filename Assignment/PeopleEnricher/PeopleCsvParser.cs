using System.Net.Mail;
using System.Text;

namespace PeopleEnricher;

/// <summary>
/// Parses and validates people CSV data.
/// </summary>
public static class PeopleCsvParser
{
    private const string ExpectedHeader =
        "FirstName,LastName,EmailAddress,ZipCode";

    /// <summary>
    /// Parses a CSV document and separates valid and invalid people.
    /// </summary>
    /// <param name="csv">The downloaded CSV document.</param>
    /// <returns>The valid people and rejected records.</returns>
    /// <exception cref="InvalidDataException">
    /// Thrown when the CSV is missing or its header is invalid.
    /// </exception>
    public static PeopleCsvParseResult Parse(string csv)
    {
        ArgumentNullException.ThrowIfNull(csv);

        string[] lines = ReadLines(csv);

        if (lines.Length == 0)
        {
            throw new InvalidDataException(
                "The people CSV is empty.");
        }

        string header = lines[0];

        // UTF-8 BOM is a file encoding marker, not part of the header.
        if (header.StartsWith('\uFEFF'))
        {
            header = header[1..];
        }

        if (!string.Equals(
                header,
                ExpectedHeader,
                StringComparison.Ordinal))
        {
            throw new InvalidDataException(
                "The people CSV header is invalid.");
        }

        ParsedRow[] parsedRows = lines
            .Skip(1)
            .Select((rawLine, index) =>
                ParseRow(rawLine, index + 2))
            .ToArray();

        Person[] people = parsedRows
            .Where(row => row.Person is not null)
            .Select(row => row.Person!)
            .ToArray();

        RejectedPerson[] rejections = parsedRows
            .Where(row => row.Rejection is not null)
            .Select(row => row.Rejection!)
            .ToArray();

        return new PeopleCsvParseResult(
            Array.AsReadOnly(people),
            Array.AsReadOnly(rejections));
    }

    private static string[] ReadLines(string csv)
    {
        using StringReader reader = new(csv);

        List<string> lines = [];

        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            lines.Add(line);
        }

        return lines.ToArray();
    }

    private static ParsedRow ParseRow(
        string rawLine,
        int lineNumber)
    {
        if (!TryParseCsvLine(
                rawLine,
                out string[] fields,
                out string parsingError))
        {
            return Reject(lineNumber, rawLine, parsingError);
        }

        if (fields.Length != 4)
        {
            return Reject(
                lineNumber,
                rawLine,
                $"Expected 4 columns but found {fields.Length}.");
        }

        string? validationError =
            ValidateName(fields[0], "FirstName") ??
            ValidateName(fields[1], "LastName") ??
            ValidateEmail(fields[2]) ??
            ValidateZipCode(fields[3]);

        if (validationError is not null)
        {
            return Reject(lineNumber, rawLine, validationError);
        }

        Person person = new(
            fields[0],
            fields[1],
            fields[2],
            fields[3]);

        return new ParsedRow(person, null);
    }

    private static ParsedRow Reject(
        int lineNumber,
        string rawLine,
        string reason)
    {
        RejectedPerson rejection = new(
            RejectionStage.PeopleCsv,
            lineNumber,
            rawLine,
            reason);

        return new ParsedRow(null, rejection);
    }

    private static string? ValidateName(
        string value,
        string fieldName)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return $"{fieldName} is required.";
        }

        if (!string.Equals(
                value,
                value.Trim(),
                StringComparison.Ordinal))
        {
            return $"{fieldName} contains surrounding whitespace.";
        }

        if (value.Any(char.IsControl))
        {
            return $"{fieldName} contains invalid control characters.";
        }

        return null;
    }

    private static string? ValidateEmail(string emailAddress)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
        {
            return "EmailAddress is required.";
        }

        if (!string.Equals(
                emailAddress,
                emailAddress.Trim(),
                StringComparison.Ordinal))
        {
            return "EmailAddress contains surrounding whitespace.";
        }

        if (!MailAddress.TryCreate(
                emailAddress,
                out MailAddress? parsedAddress) ||
            parsedAddress is null ||
            !string.Equals(
                parsedAddress.Address,
                emailAddress,
                StringComparison.OrdinalIgnoreCase))
        {
            return "EmailAddress is invalid.";
        }

        return null;
    }

    private static string? ValidateZipCode(string zipCode)
    {
        if (zipCode.Length != 5 ||
            zipCode.Any(character =>
                character < '0' || character > '9'))
        {
            return "ZipCode must contain exactly five numeric digits.";
        }

        return null;
    }

    private static bool TryParseCsvLine(
        string line,
        out string[] fields,
        out string error)
    {
        List<string> parsedFields = [];

        int index = 0;

        while (true)
        {
            if (index < line.Length && line[index] == '"')
            {
                index++;

                StringBuilder field = new();

                bool closingQuoteFound = false;

                while (index < line.Length)
                {
                    if (line[index] == '"')
                    {
                        if (index + 1 < line.Length &&
                            line[index + 1] == '"')
                        {
                            field.Append('"');
                            index += 2;
                        }
                        else
                        {
                            index++;
                            closingQuoteFound = true;
                            break;
                        }
                    }
                    else
                    {
                        field.Append(line[index]);
                        index++;
                    }
                }

                if (!closingQuoteFound)
                {
                    fields = [];
                    error = "A quoted CSV field was not closed.";
                    return false;
                }

                if (index < line.Length && line[index] != ',')
                {
                    fields = [];
                    error = "Unexpected characters after a quoted field.";
                    return false;
                }

                parsedFields.Add(field.ToString());
            }
            else
            {
                int start = index;

                while (index < line.Length && line[index] != ',')
                {
                    if (line[index] == '"')
                    {
                        fields = [];
                        error = "Unexpected quotation mark in an unquoted field.";
                        return false;
                    }

                    index++;
                }

                parsedFields.Add(line[start..index]);
            }

            if (index == line.Length)
            {
                break;
            }

            // Move past the comma.
            index++;

            if (index == line.Length)
            {
                parsedFields.Add(string.Empty);
                break;
            }
        }

        fields = parsedFields.ToArray();
        error = string.Empty;

        return true;
    }

    private sealed record ParsedRow(
        Person? Person,
        RejectedPerson? Rejection);
}