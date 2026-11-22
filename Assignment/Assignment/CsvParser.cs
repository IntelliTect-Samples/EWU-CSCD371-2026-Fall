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
        string[] f = [.. row.Split(',').Select(s => s.Trim())];
        if (f.Length != 4) return (null, "Wrong number of columns");
        if (f[0].Length == 0 || f[1].Length == 0) return (null, "Missing name");
        if (!MailAddress.TryCreate(f[2], out MailAddress? mail) || mail.Address != f[2]) return (null, "Invalid email");
        if (f[3].Length != 5 || !f[3].All(char.IsAsciiDigit)) return (null, "Invalid ZIP");
        return (new Person(f[0], f[1], f[2], f[3]), null);
    }

    public static Location? ParseLocation(string line)
    {
        string[] f = [.. line.Trim().Split(',').Select(s => s.Trim().Trim('"'))];
        return f.Length == 2
            && f[0].Length > 0 && f[0].All(c => char.IsLetter(c) || c is ' ' or '-' or '\'' or '.')
            && f[1].Length == 2 && f[1].All(char.IsAsciiLetterUpper)
            ? new Location(f[0], f[1]) : null;
    }
}
