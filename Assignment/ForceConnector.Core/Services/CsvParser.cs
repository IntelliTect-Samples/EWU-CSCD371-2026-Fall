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
        ArgumentException.ThrowIfNullOrWhiteSpace(csv);

        string[] lines =
            csv.Split(
                Environment.NewLine,
                StringSplitOptions.RemoveEmptyEntries);

        if (lines.Length < 2)
        {
            throw new InvalidDataException(
                "CSV does not contain any data rows.");
        }

        const string ExpectedHeader =
            "FirstName,LastName,EmailAddress,ZipCode";

        string header = lines[0].Trim();

        if (!header.Equals(
                ExpectedHeader,
                StringComparison.OrdinalIgnoreCase))
        {
            throw new InvalidDataException(
                "CSV header is invalid.");
        }

        List<Person> people = [];

        foreach (string line in lines.Skip(1))
        {
            string[] parts = line.Split(',');

            if (parts.Length != 4)
            {
                continue;
            }

            Person person = new()
            {
                FirstName = parts[0].Trim(),

                LastName = parts[1].Trim(),

                EmailAddress = parts[2].Trim(),

                ZipCode = parts[3].Trim()
            };

            people.Add(person);
        }

        return people;
    }
}