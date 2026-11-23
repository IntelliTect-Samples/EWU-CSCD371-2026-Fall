using System.Text.RegularExpressions;

namespace Assignment9;

/// <summary>
/// Parses the body of <c>GET /people/zip/{zip}</c>, for example <c>"SPOKANE","WA"</c>.
/// The body is invalid (and <see cref="InvalidResponseException"/> is thrown) unless it is a single
/// CSV line with exactly two fields, where the city is 1-60 characters of letters, spaces,
/// apostrophes, periods and hyphens starting with a letter, and the state is an upper-case USPS
/// code for a state, DC, a territory, or a military "state".
/// </summary>
public static partial class LocationCsv
{
    private const int maxCityLength = 60;

    private static readonly HashSet<string> stateCodes = new(StringComparer.Ordinal)
    {
        "AL", "AK", "AZ", "AR", "CA", "CO", "CT", "DE", "FL", "GA",
        "HI", "ID", "IL", "IN", "IA", "KS", "KY", "LA", "ME", "MD",
        "MA", "MI", "MN", "MS", "MO", "MT", "NE", "NV", "NH", "NJ",
        "NM", "NY", "NC", "ND", "OH", "OK", "OR", "PA", "RI", "SC",
        "SD", "TN", "TX", "UT", "VT", "VA", "WA", "WV", "WI", "WY",
        "DC", "PR", "GU", "VI", "AS", "MP", "AA", "AE", "AP",
    };

    public static Location Parse(string body)
    {
        ArgumentNullException.ThrowIfNull(body);

        string line = body.Trim();
        if (line.Length == 0)
        {
            throw new InvalidResponseException("The location response was empty.");
        }

        if (line.Contains('\n', StringComparison.Ordinal))
        {
            throw new InvalidResponseException("The location response had more than one line.");
        }

        if (!CsvLine.TryParse(line, out IReadOnlyList<string> fields) || fields.Count != 2)
        {
            throw new InvalidResponseException($"The location response was not a \"City\",\"State\" pair: '{line}'.");
        }

        string city = fields[0];
        string state = fields[1];

        if (city.Length > maxCityLength || !CityPattern().IsMatch(city))
        {
            throw new InvalidResponseException($"The location response had an invalid city: '{city}'.");
        }

        if (!stateCodes.Contains(state))
        {
            throw new InvalidResponseException($"The location response had an invalid state: '{state}'.");
        }

        return new Location(city, state);
    }

    [GeneratedRegex(@"^\p{L}([\p{L}\p{M}' .\-]*[\p{L}\p{M}.])?$")]
    private static partial Regex CityPattern();
}
