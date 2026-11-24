using ForceConnector.Core.Interfaces;
using ForceConnector.Core.Models;

namespace ForceConnector.Core.Services;

/// <summary>
/// Parses CSV data returned by the people endpoint.
/// </summary>
public sealed class CsvParser : ICsvParser
{
    public IReadOnlyList<Person> ParsePeople(string csv)
    {
        return ParsePeopleWithRejections(csv).People;
    }

    public CsvParseResult ParsePeopleWithRejections(string csv)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(csv);

        IReadOnlyList<IReadOnlyList<string>> records =
            CsvRecordParser.Parse(csv);

        if (records.Count < 2)
        {
            throw new InvalidDataException(
                "CSV does not contain any data rows.");
        }

        string[] expectedHeader =
        [
            "FirstName",
            "LastName",
            "EmailAddress",
            "ZipCode"
        ];

        if (!records[0].SequenceEqual(
                expectedHeader,
                StringComparer.OrdinalIgnoreCase))
        {
            throw new InvalidDataException("CSV header is invalid.");
        }

        List<Person> people = [];
        List<Rejection> rejections = [];

        foreach (IReadOnlyList<string> fields in records.Skip(1))
        {
            if (fields.Count != expectedHeader.Length)
            {
                rejections.Add(
                    new Rejection
                    {
                        EmailAddress = fields.Count > 2
                            ? fields[2].Trim()
                            : string.Empty,
                        Reason =
                            $"Malformed CSV row. " +
                            $"Expected {expectedHeader.Length} columns " +
                            $"but found {fields.Count}. " +
                            $"Row contents: " +
                            $"{string.Join(" | ", fields)}"
                    });
                continue;
            }

            people.Add(
                new Person
                {
                    FirstName = fields[0].Trim(),
                    LastName = fields[1].Trim(),
                    EmailAddress = fields[2].Trim(),
                    ZipCode = fields[3].Trim()
                });
        }

        return new CsvParseResult
        {
            People = people,
            Rejections = rejections
        };
    }
}