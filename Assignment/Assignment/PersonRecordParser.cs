using System.Globalization;

namespace Assignment;

/// <summary>Parses a CSV record into a Person.</summary>
/// <remarks>
/// Format: Id,FirstName,LastName,Email,StreetAddress,City,State,Zip
/// Validation rules (each violation throws FormatException):
/// 1. The record must have exactly 8 fields.
/// 2. Id must be a positive integer.
/// 3. All fields other than Id are required (not empty or whitespace).
/// </remarks>
public static class PersonRecordParser
{
    public const string Header = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";
    private static readonly string[] _FieldNames = Header.Split(',');

    public static Person Parse(string line)
    {
        ArgumentNullException.ThrowIfNull(line);

        string[] fields = line.Split(',');
        if (fields.Length != _FieldNames.Length)
        {
            throw new FormatException($"Expected {_FieldNames.Length} fields, but got {fields.Length}.");
        }

        int id = int.Parse(fields[0], NumberStyles.None, CultureInfo.InvariantCulture);
        if (id == 0)
        {
            throw new FormatException("Id cannot be zero.");
        }

        for (int i = 1; i < fields.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(fields[i]))
            {
                throw new FormatException($"Field '{_FieldNames[i]}' cannot be null or empty.");
            }
        }

        return new Person(
            id,
            fields[1],
            fields[2],
            new Address(fields[4], fields[5], fields[6], fields[7]),
            fields[3]
        );

    }
}