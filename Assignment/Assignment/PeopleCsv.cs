namespace Assignment9;

/// <summary>The valid people from a people CSV, plus every row that was removed.</summary>
public sealed record PeopleParseResult(IReadOnlyList<Person> People, IReadOnlyList<Rejection> Rejections);

/// <summary>
/// Parses the body of <c>GET /people/{user}</c>.
/// <para>
/// Problems with the <em>response as a whole</em> throw <see cref="InvalidResponseException"/> so the
/// download is retried: an empty body, a missing or wrong header, or a body with no valid people.
/// Problems with an <em>individual row</em> remove just that person (see <see cref="PersonValidator"/>).
/// </para>
/// </summary>
public static class PeopleCsv
{
    public const string ExpectedHeader = "FirstName,LastName,EmailAddress,ZipCode";

    public static PeopleParseResult Parse(string csv)
    {
        ArgumentNullException.ThrowIfNull(csv);

        string[] lines =
        [
            .. csv.Split('\n')
                .Select(line => line.TrimEnd('\r'))
                .Where(line => !string.IsNullOrWhiteSpace(line)),
        ];

        if (lines.Length == 0)
        {
            throw new InvalidResponseException("The people CSV was empty.");
        }

        string header = lines[0].TrimStart('﻿');
        if (!string.Equals(header, ExpectedHeader, StringComparison.Ordinal))
        {
            throw new InvalidResponseException($"The people CSV had an unexpected header: '{header}'.");
        }

        PersonValidationResult[] rows =
        [
            .. lines
                .Skip(1)
                .Select((line, index) => PersonValidator.Validate(line, rowNumber: index + 2)),
        ];

        List<Person> people = [.. rows.Select(row => row.Person).OfType<Person>()];
        if (people.Count == 0)
        {
            throw new InvalidResponseException($"The people CSV contained no valid people ({rows.Length} data rows).");
        }

        List<Rejection> rejections = [.. rows.Select(row => row.Rejection).OfType<Rejection>()];
        return new PeopleParseResult(people, rejections);
    }
}
