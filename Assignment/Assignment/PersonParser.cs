using System.Text.RegularExpressions;

namespace Assignment;

public sealed record Rejection(int LineNumber, string Row, string Reason);
public sealed class PersonParseResultReport
{
    public required IReadOnlyList<Person> People { get; init; }
    public required IReadOnlyList<Rejection> Rejections { get; init; }

    public int RejectedRowCount => Rejections.Select(r => r.LineNumber).Distinct().Count();

    public IEnumerable<IGrouping<string, Rejection>> RejectionsByReason =>
        Rejections.GroupBy(r => r.Reason);
}
public static partial class PersonParser
{
    internal sealed class Rule
    {
        public required string ErrorMessage { get; init; }
        public required Func<string[], bool> Predicate { get; init; }
    }

    [GeneratedRegex(@"^[^@]+@[^@]+\.[^@]+$")]
    private static partial Regex ValidEmail();

    private static readonly Rule[] Rules =
    [
        new()
        {
            ErrorMessage = "The first name is required",
            Predicate = fields => !string.IsNullOrWhiteSpace(fields[0])
        },
        new()
        {
            ErrorMessage = "The last name is required",
            Predicate = fields => !string.IsNullOrWhiteSpace(fields[1])
        },
        new()
        {
            ErrorMessage = "The email is required",
            Predicate = fields => !string.IsNullOrWhiteSpace(fields[2])
        },
        new()
        {
            ErrorMessage = "The email is must have a valid format",
            Predicate = fields => ValidEmail().IsMatch(fields[2])
        },
        new()
        {
            ErrorMessage = "The zip code is required",
            Predicate = fields => !string.IsNullOrWhiteSpace(fields[3])
        },
        new()
        {
            ErrorMessage = "The zip code must be 5 digits",
            Predicate=fields => fields[3].Length == 5
        }
    ];

    private static string[] SplitFields(string line)
    {
        return line.Split(',', StringSplitOptions.TrimEntries);
    }

    private static List<string> ValidateFields(string[] fields)
    {
        if (fields.Length != 4)
        {
            return ["4 fields expected"];
        }

        List<string> errors = Rules
            .Where(rule => !rule.Predicate(fields))
            .Select(rule => rule.ErrorMessage).ToList();

        return errors;
    }

    private static Person CreatePerson(int lineNumber, string[] fields)
    {
        return new Person(lineNumber, fields[0], fields[1], fields[2], fields[3]);
    }

    /// <summary>
    /// Rarses a line into a Person object.
    /// Returns (true, null) if the string is valid.
    /// Returns (false, reason) if the string is invalid.
    /// Rules:
    /// - email must have exactly 1 @ symbol folowed by at least one . 
    ///     example: email@website.com
    /// - zipcode must be 5 digits
    /// - all fields must have at least 1 character
    /// </summary>
    /// <param name="line"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static PersonParseResultReport ParseCsv(string csv)
    {
        var rows = csv
            .Split('\n', StringSplitOptions.TrimEntries)
            .Select((line, index) => (Line: line, LineNumber: index + 1))
            .Skip(1)
            .Where(r => !string.IsNullOrWhiteSpace(r.Line))
            .Select(r =>
            {
                string[] fields = SplitFields(r.Line);
                return (r.Line, r.LineNumber, Fields: fields, Errors: ValidateFields(fields));
            })
            .ToList();

        var people = rows.Where(row => row.Errors.Count == 0).Select(row => CreatePerson(row.LineNumber, row.Fields)).ToList();

        var rejections = rows.Where(row => row.Errors.Count > 0).SelectMany(row => row.Errors.Select(error => new Rejection(row.LineNumber, row.Line, error))).ToList();

        return new PersonParseResultReport { People = people, Rejections = rejections };
    }


}
