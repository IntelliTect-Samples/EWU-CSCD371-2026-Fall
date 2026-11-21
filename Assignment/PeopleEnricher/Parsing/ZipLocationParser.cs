using System.Text.RegularExpressions;

namespace PeopleEnricher;

/// <summary>
/// Parses and validates city and state responses from the ZIP endpoint.
/// </summary>
public static partial class ZipLocationParser
{
    private static readonly Regex ResponsePattern = new(
        "^\"(?<city>(?:[^\"]|\"\")*)\",\"(?<state>(?:[^\"]|\"\")*)\"$",
        RegexOptions.CultureInvariant);

    /// <summary>
    /// Parses a ZIP lookup response into a city and state.
    /// </summary>
    /// <param name="response">The raw CSV response.</param>
    /// <returns>The validated city and state.</returns>
    /// <exception cref="InvalidDataException">
    /// Thrown when the response is empty or malformed.
    /// </exception>
    public static ZipLocation Parse(string response)
    {
        ArgumentNullException.ThrowIfNull(response);

        string line = response.TrimEnd('\r', '\n');

        Match match = ResponsePattern.Match(line);

        if (!match.Success)
        {
            throw new InvalidDataException(
                "ZIP lookup response must contain quoted city and state fields.");
        }

        string city = match.Groups["city"].Value.Replace(
            "\"\"",
            "\"",
            StringComparison.Ordinal);

        string state = match.Groups["state"].Value;

        if (string.IsNullOrWhiteSpace(city) ||
            !string.Equals(city, city.Trim(), StringComparison.Ordinal) ||
            city.Any(char.IsControl))
        {
            throw new InvalidDataException(
                "ZIP lookup returned an invalid city.");
        }

        if (state.Length != 2 ||
            state.Any(character => !char.IsAsciiLetterUpper(character)))
        {
            throw new InvalidDataException(
                "ZIP lookup returned an invalid state abbreviation.");
        }

        return new ZipLocation(city, state);
    }
}