using System.Text.RegularExpressions;

namespace Assignment9;

/// <summary>The outcome of validating one people-CSV row: exactly one of the two is set.</summary>
public sealed record PersonValidationResult(Person? Person, Rejection? Rejection);

/// <summary>
/// Decides whether one row of the people CSV is a valid person.
/// A row is invalid (and the person removed) when any of these is true:
/// <list type="bullet">
/// <item>The line is not well-formed CSV (for example an unterminated quote).</item>
/// <item>It does not have exactly 4 fields.</item>
/// <item>FirstName or LastName is empty, longer than 50 characters, has leading/trailing
/// whitespace, or contains anything other than letters, spaces, apostrophes, hyphens and periods
/// (it must start with a letter).</item>
/// <item>EmailAddress is not a plain <c>local@domain.tld</c> address.</item>
/// <item>ZipCode is not exactly five ASCII digits.</item>
/// </list>
/// Values are never trimmed or otherwise repaired.
/// </summary>
public static partial class PersonValidator
{
    private const int maxNameLength = 50;

    public static PersonValidationResult Validate(string line, int rowNumber)
    {
        ArgumentNullException.ThrowIfNull(line);
        string subject = $"Row {rowNumber}: {line}";

        if (!CsvLine.TryParse(line, out IReadOnlyList<string> fields))
        {
            return Reject(subject, RejectionReasons.MalformedRow, "The row could not be parsed as CSV.");
        }

        if (fields.Count != 4)
        {
            return Reject(subject, RejectionReasons.WrongFieldCount, $"Expected 4 fields but found {fields.Count}.");
        }

        string firstName = fields[0];
        string lastName = fields[1];
        string email = fields[2];
        string zip = fields[3];

        if (!IsValidName(firstName))
        {
            return Reject(subject, RejectionReasons.InvalidFirstName, $"'{firstName}' is not a valid name.");
        }

        if (!IsValidName(lastName))
        {
            return Reject(subject, RejectionReasons.InvalidLastName, $"'{lastName}' is not a valid name.");
        }

        if (!IsValidEmail(email))
        {
            return Reject(subject, RejectionReasons.InvalidEmail, $"'{email}' is not a valid email address.");
        }

        if (!IsValidZipCode(zip))
        {
            return Reject(subject, RejectionReasons.InvalidZipCode, $"'{zip}' is not a 5-digit ZIP code.");
        }

        return new PersonValidationResult(new Person(firstName, lastName, email, zip), null);
    }

    public static bool IsValidName(string value) =>
        value.Length is > 0 and <= maxNameLength && NamePattern().IsMatch(value);

    public static bool IsValidEmail(string value) =>
        EmailPattern().IsMatch(value) && !value.Contains("..", StringComparison.Ordinal);

    public static bool IsValidZipCode(string value) => ZipPattern().IsMatch(value);

    private static PersonValidationResult Reject(string subject, string reason, string detail) =>
        new(null, new Rejection(subject, reason, detail));

    // Starts with a letter; then letters, combining marks, spaces, apostrophes, periods, hyphens.
    // Must not end with a space, so leading/trailing whitespace is rejected.
    [GeneratedRegex(@"^\p{L}([\p{L}\p{M}' .\-]*[\p{L}\p{M}'.])?$")]
    private static partial Regex NamePattern();

    [GeneratedRegex(@"^[A-Za-z0-9._%+\-]+@[A-Za-z0-9\-]+(\.[A-Za-z0-9\-]+)*\.[A-Za-z]{2,}$")]
    private static partial Regex EmailPattern();

    [GeneratedRegex(@"^[0-9]{5}$")]
    private static partial Regex ZipPattern();
}
