using System.Net.Mail;

namespace Assignment;

public sealed record Location(string City, string State);

/* 
 * Turns one line of text into data. A person is invalid when the row does not have exactly
 * 4 columns, a name is empty, the email is not a plain address, or the ZIP is not 5 digits.
 * A location is invalid unless it is "City","ST" with a non-empty city and 2 uppercase letters.
 * Invalid rows are rejected with a reason, never repaired.
*/ 
public static class CsvParser
{
    public static (Person? Person, string? Reason) ParsePerson(string row)
    {
        string[] f = row.Split(',');
        if (f.Length != 4) return (null, "Wrong number of columns");
        if (f[0].Length == 0 || f[0] != f[0].Trim()
            || f[1].Length == 0 || f[1] != f[1].Trim()) return (null, "Missing name");
        if (f[2].Any(char.IsWhiteSpace)
            || !MailAddress.TryCreate(f[2], out MailAddress? mail)
            || mail.Address != f[2]) return (null, "Invalid email");
        if (f[3].Length != 5 || !f[3].All(char.IsAsciiDigit)) return (null, "Invalid ZIP");
        return (new Person(f[0], f[1], f[2], f[3]), null);
    }

    public static Location? ParseLocation(string line)
    {
        string[] f = SplitLocationCsv(line.TrimEnd('\r', '\n'));
        if (f.Length != 2) return null;

        string city = f[0];
        string state = f[1];
        return city.Trim().Length > 0
            && city.All(c => char.IsLetter(c) || c is ' ' or '-' or '\'' or '.' or ',')
            && state.Length == 2
            && state.All(char.IsAsciiLetterUpper)
            ? new Location(city, state)
            : null;
    }

    private static string[] SplitLocationCsv(string line)
    {
        var fields = new List<string>();
        var field = new System.Text.StringBuilder();
        bool quoted = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (quoted && i + 1 < line.Length && line[i + 1] == '"')
                {
                    field.Append('"');
                    i++;
                }
                else
                {
                    quoted = !quoted;
                }
            }
            else if (c == ',' && !quoted)
            {
                fields.Add(field.ToString().Trim());
                field.Clear();
            }
            else
            {
                field.Append(c);
            }
        }

        if (quoted) return [];
        fields.Add(field.ToString().Trim());
        return [.. fields];
    }
}
