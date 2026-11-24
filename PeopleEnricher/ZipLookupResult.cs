namespace PeopleEnricher;

/// <summary>
/// Represents the outcome of obtaining location information for a ZipCode.
///
/// A successful lookup populates <see cref="Location"/>
/// and leaves <see cref="FailureReason"/> as <see langword="null"/>.
///
/// A failed lookup populates <see cref="FailureReason"/>
/// and leaves <see cref="Location"/> as <see langword="null"/>.
/// </summary>
/// <param name="Location">
/// The formatted location in the form "City,State".
/// This value is <see langword="null"/> when the lookup fails.
/// </param>
/// <param name="FailureReason">
/// A human-readable explanation describing why the lookup failed.
/// This value is <see langword="null"/> when the lookup succeeds.
/// </param>
public record ZipLookupResult(
    string? Location,
    string? FailureReason);