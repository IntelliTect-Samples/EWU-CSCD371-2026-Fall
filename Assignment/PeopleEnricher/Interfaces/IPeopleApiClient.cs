namespace PeopleEnricher;

/// <summary>
/// Defines the API operations needed by the enrichment service.
/// </summary>
public interface IPeopleApiClient
{
    Task<PeopleCsvParseResult> GetPeopleAsync(
        string user,
        CancellationToken cancellationToken = default);

    Task<ZipLocation> GetZipLocationAsync(
        string zipCode,
        CancellationToken cancellationToken = default);
}