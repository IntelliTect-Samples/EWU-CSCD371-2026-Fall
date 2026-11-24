namespace Assignment.Interfaces;

/// <summary>Represents one person parsed from the source CSV.</summary>
/// <param name="FirstName">The person's first name.</param>
/// <param name="LastName">The person's last name.</param>
/// <param name="EmailAddress">The person's email address.</param>
/// <param name="ZipCode">The person's ZIP code.</param>
public sealed record Person(string FirstName, string LastName, string EmailAddress, string ZipCode);

/// <summary>Represents a person and the location attached to it.</summary>
/// <param name="FirstName">The person's first name.</param>
/// <param name="LastName">The person's last name.</param>
/// <param name="EmailAddress">The person's email address.</param>
/// <param name="ZipCode">The person's ZIP code.</param>
/// <param name="City">The person's city.</param>
/// <param name="State">The person's state abbreviation.</param>
public sealed record EnrichedPerson(
    string FirstName,
    string LastName,
    string EmailAddress,
    string ZipCode,
    string City,
    string State);

/// <summary>Describes a source row that could not be enriched.</summary>
/// <param name="RowDescription">A description of the rejected row.</param>
/// <param name="Reason">The reason the row was rejected.</param>
public sealed record Rejection(string RowDescription, string Reason);

/// <summary>Summarizes the people downloaded, rejected, and written for one run.</summary>
/// <param name="TotalPeople">The number of source people processed.</param>
/// <param name="RejectedPeople">The number of rejected people.</param>
/// <param name="EnrichedPeople">The number of successfully enriched people.</param>
/// <param name="Rejections">The rejection details.</param>
public sealed record RunResult(int TotalPeople, int RejectedPeople, int EnrichedPeople, IReadOnlyList<Rejection> Rejections)
{
    /// <summary>Groups rejection reasons for concise reporting.</summary>
    /// <returns>The rejection counts, ordered by reason.</returns>
    public IReadOnlyList<string> FormatRejectionSummary()
    {
        return Rejections
            .GroupBy(rejection => rejection.Reason, StringComparer.Ordinal)
            .OrderBy(group => group.Key, StringComparer.Ordinal)
            .Select(group => $"{group.Key}: {group.Count()}")
            .ToArray();
    }
}

/// <summary>Indicates that the people data could not be downloaded after retrying.</summary>
public sealed class PeopleDownloadException : Exception
{
    /// <summary>Initializes a people download exception.</summary>
    /// <param name="message">The error message.</param>
    /// <param name="innerException">The last exception encountered, if any.</param>
    public PeopleDownloadException(string message, Exception? innerException = null)
        : base(message, innerException)
    {
    }
}
