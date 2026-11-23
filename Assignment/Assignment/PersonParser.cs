using System.Text.RegularExpressions;

namespace Assignment;

public sealed record ParseResult(Person? Person, IReadOnlyList<string> Errors)
{
    public static ParseResult Accepted(Person person)
    {
        return new(person, []);
    }

    public static ParseResult Rejected(IReadOnlyList<string> errors)
    {
        return new(null, errors);
    }

    public bool IsValid => Person is not null && Errors.Count == 0;

}


public static partial class PersonParser
{
    internal class Rule
    {
        public required string ErrorMessage { get; init; }
        public required Func<string[], bool> Predicate { get; init; }
    }

    [GeneratedRegex(@"^[^@]*@[^@]*\.[^@]*$")]
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

    private static IReadOnlyList<string> ValidateFields(string[] fields)
    {
        if (fields.Length != 4)
        {
            return ["4 fields expected"];
        }

        List<string> errors = [];

        foreach (var rule in Rules)
        {
            if (!rule.Predicate(fields))
            {
                errors.Add(rule.ErrorMessage);
            }
        }

        return errors;
    }

    private static Person CreatePerson(string[] fields)
    {
        return new Person(fields[0], fields[1], fields[2], fields[3]);
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
    public static ParseResult Parse(string line)
    {
        string[] fields = SplitFields(line);
        var errors = ValidateFields(fields);

        if (errors.Count == 0)
        {
            return ParseResult.Accepted(CreatePerson(fields));
        }

        else
        {
            return ParseResult.Rejected(errors);
        }
    }


}
