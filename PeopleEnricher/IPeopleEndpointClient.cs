namespace PeopleEnricher;

public interface IPeopleEndpointClient
{
    Task<string> DownloadPeopleCsvAsync(
        CancellationToken cancellationToken = default);
    
    Task<string> ZipCodeLookupAsync(
        string zipCode,
        CancellationToken cancellationToken = default);
}