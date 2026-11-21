using System.Text.RegularExpressions;

namespace Assignment;

public partial class Importer
{

    [GeneratedRegex(@"^[^@]*@[^@]*\.[^@]*$")]
    private static partial Regex ValidEmail();

    private readonly TextWriter _writer;

    Importer(TextWriter writer = null!)
        => _writer = writer ?? Console.Out;

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
    private static (bool, string?) ValidateAndParse(string line, out Person? result)
    {
        result = null;
        var rawTokens = line.Split(',', StringSplitOptions.TrimEntries);

        string[] fieldNames =
        [
            "firstName",
            "lastName",
            "email",
            "zip",
            "city"
        ];

        if (rawTokens.Length != fieldNames.Length)
        {
            return (false, "Each line is expected to have 5 fields");
        }

        for (int i = 0; i < fieldNames.Length; ++i)
        {
            if (string.IsNullOrWhiteSpace(rawTokens[i]))
            {
                return (false, $"{fieldNames[i]} must have at least 1 character");
            }
        }

        string firstName = rawTokens[0];
        string lastName = rawTokens[1];
        string email = rawTokens[2];
        string zip = rawTokens[3];

        if (!ValidEmail().IsMatch(email))
        {
            return (false, $"{email} is not in a valid email format");
        }

        if (zip.Length != 5)
        {
            return (false, $"{zip} is not a valid US zipcode. Must be 5 digits");
        }

        result = new Person(firstName, lastName, email, zip);
        return (true, null);
    }



}