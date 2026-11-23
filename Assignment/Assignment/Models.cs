namespace Assignment9;

/// <summary>A person as downloaded from <c>GET /people/{user}</c>.</summary>
public sealed record Person(string FirstName, string LastName, string EmailAddress, string ZipCode)
{
    /// <summary>A short, human-readable description used in rejection reports.</summary>
    public string Describe() => $"{FirstName} {LastName} <{EmailAddress}> (ZIP {ZipCode})";
}

/// <summary>A city and state as returned by <c>GET /people/zip/{zip}</c>.</summary>
public sealed record Location(string City, string State);

/// <summary>A person whose ZIP code was successfully resolved to a city and state.</summary>
public sealed record EnrichedPerson(Person Person, Location Location)
{
    public const string CsvHeader = "FirstName,LastName,EmailAddress,ZipCode,City,State";

    public string ToCsvLine() => CsvLine.Format(
        Person.FirstName,
        Person.LastName,
        Person.EmailAddress,
        Person.ZipCode,
        Location.City,
        Location.State);
}

/// <summary>A person (or raw row) that was removed from the output, and why.</summary>
/// <param name="Subject">The row or person that was removed.</param>
/// <param name="Reason">A short category from <see cref="RejectionReasons"/>, used for grouping.</param>
/// <param name="Detail">Extra detail about the specific failure.</param>
public sealed record Rejection(string Subject, string Reason, string Detail);

/// <summary>How many people were removed for a given reason.</summary>
public sealed record RejectionCount(string Reason, int Count);

/// <summary>The reasons a person can be removed. Kept as constants so reports group cleanly.</summary>
public static class RejectionReasons
{
    public const string MalformedRow = "Malformed CSV row";
    public const string WrongFieldCount = "Wrong number of fields";
    public const string InvalidFirstName = "Invalid first name";
    public const string InvalidLastName = "Invalid last name";
    public const string InvalidEmail = "Invalid email address";
    public const string InvalidZipCode = "Invalid ZIP code";
    public const string LookupInvalidData = "Location lookup returned invalid data";
    public const string LookupTimedOut = "Location lookup timed out";
    public const string LookupHttpError = "Location lookup failed";
}
