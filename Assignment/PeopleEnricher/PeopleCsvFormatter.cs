namespace PeopleEnricher;

public sealed class PeopleCsvFormatter : IPeopleCsvFormatter
{
    private const string Header =
        "FirstName,LastName,EmailAddress,ZipCode,City,State";

    private static readonly char[] SpecialCharacters = [',', '"', '\r', '\n'];

    public string Format(IEnumerable<EnrichedPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        IEnumerable<string> rows = people.Select(person => string.Join(
            ',',
            Escape(person.FirstName),
            Escape(person.LastName),
            Escape(person.EmailAddress),
            Escape(person.ZipCode),
            Escape(person.City),
            Escape(person.State)));

        return string.Join("\n", rows.Prepend(Header)) + "\n";
    }

    // RFC 4180: quote a field only if it needs it, and double any quotes inside.
    private static string Escape(string value) =>
        value.IndexOfAny(SpecialCharacters) >= 0
            ? $"\"{value.Replace("\"", "\"\"", StringComparison.Ordinal)}\""
            : value;
}