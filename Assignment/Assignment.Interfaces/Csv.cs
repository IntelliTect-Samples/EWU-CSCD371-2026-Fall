using System.Globalization;
using System.Text;

namespace Assignment.Interfaces;

/// <summary>Parses and formats CSV data and validates people and locations.</summary>
public static class Csv
{
    /// <summary>Parses CSV content, including quoted commas, escaped quotes, and embedded line breaks.</summary>
    /// <param name="csv">The CSV content to parse.</param>
    /// <returns>The parsed rows, each represented as an array of fields.</returns>
    /// <exception cref="InvalidDataException">A quoted field is not terminated.</exception>
    public static IReadOnlyList<string[]> Parse(string csv)
    {
        var rows = new List<string[]>();
        var row = new List<string>();
        var field = new StringBuilder();
        var inQuotes = false;

        for (var i = 0; i < csv.Length; i++)
        {
            var character = csv[i];
            if (character == '"')
            {
                if (inQuotes && i + 1 < csv.Length && csv[i + 1] == '"')
                {
                    field.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (character == ',' && !inQuotes)
            {
                row.Add(field.ToString());
                field.Clear();
            }
            else if ((character == '\r' || character == '\n') && !inQuotes)
            {
                if (character == '\r' && i + 1 < csv.Length && csv[i + 1] == '\n')
                {
                    i++;
                }

                row.Add(field.ToString());
                field.Clear();
                if (row.Count > 1 || row.Any(value => value.Length > 0))
                {
                    rows.Add(row.ToArray());
                }
                row.Clear();
            }
            else
            {
                field.Append(character);
            }
        }

        if (inQuotes)
        {
            throw new InvalidDataException("CSV contains an unterminated quoted field.");
        }

        if (field.Length > 0 || row.Count > 0)
        {
            row.Add(field.ToString());
            rows.Add(row.ToArray());
        }

        return rows;
    }


    /// <summary>Escapes a CSV field when it contains a comma, quote, or line break.</summary>
    /// <param name="value">The field value to escape.</param>
    /// <returns>The CSV-escaped field.</returns>
    public static string Escape(string value)
    {
        if (value.IndexOfAny([',', '"', '\r', '\n']) < 0)
        {
            return value;
        }

        return '"' + value.Replace("\"", "\"\"", StringComparison.Ordinal) + '"';
    }


    /// <summary>Formats and escapes a CSV header row.</summary>
    /// <param name="header">The header fields.</param>
    /// <returns>The formatted CSV header.</returns>
    public static string FormatHeader(string[] header) =>
        string.Join(',', header.Select(Escape));


    /// <summary>Formats and escapes a CSV data row.</summary>
    /// <param name="values">The row fields.</param>
    /// <returns>The formatted CSV row.</returns>
    public static string FormatRow(string[] values) =>
        string.Join(',', values.Select(Escape));


    /// <summary>Determines whether a person has a name, valid email address, and five-digit ZIP code.</summary>
    /// <param name="person">The person to validate.</param>
    /// <returns><see langword="true"/> if the person is valid; otherwise, <see langword="false"/>.</returns>
    public static bool IsValidPerson(Person person)
    {
        if (string.IsNullOrWhiteSpace(person.FirstName) ||
            string.IsNullOrWhiteSpace(person.LastName) ||
            string.IsNullOrWhiteSpace(person.EmailAddress))
        {
            return false;
        }

        var email = person.EmailAddress.Trim();
        var at = email.IndexOf('@', StringComparison.Ordinal);
        return at > 0 &&
               at == email.LastIndexOf('@') &&
               at < email.Length - 1 &&
               email[(at + 1)..].Contains('.', StringComparison.Ordinal) &&
               !email.Any(char.IsWhiteSpace) &&
               person.ZipCode.Trim().Length == 5 &&
               person.ZipCode.Trim().All(char.IsAsciiDigit);
    }


    /// <summary>Gets the first validation failure for a person.</summary>
    /// <param name="person">The person to validate.</param>
    /// <returns>The rejection reason, or <c>Invalid person</c> when no validation fails.</returns>
    public static string PersonInvalidReason(Person person)
    {
        if (string.IsNullOrWhiteSpace(person.FirstName) ||
            string.IsNullOrWhiteSpace(person.LastName))
        {
            return "Missing name";
        }

        var email = person.EmailAddress.Trim();
        var at = email.IndexOf('@', StringComparison.Ordinal);
        if (at <= 0 ||
            at != email.LastIndexOf('@') ||
            at >= email.Length - 1 ||
            !email[(at + 1)..].Contains('.', StringComparison.Ordinal) ||
            email.Any(char.IsWhiteSpace))
        {
            return "Invalid email";
        }

        var zip = person.ZipCode.Trim();
        if (zip.Length != 5 || !zip.All(char.IsAsciiDigit))
        {
            return "Invalid ZIP code";
        }

        return "Invalid person";
    }


    /// <summary>Determines whether a city and uppercase two-letter state form a valid location.</summary>
    /// <param name="city">The city name.</param>
    /// <param name="state">The state abbreviation.</param>
    /// <returns><see langword="true"/> if both location fields are valid; otherwise, <see langword="false"/>.</returns>
    public static bool IsValidLocation(string city, string state) =>
        !string.IsNullOrWhiteSpace(city) &&
        state.Length == 2 &&
        state.All(char.IsAsciiLetter) &&
        state.All(char.IsUpper);
}
