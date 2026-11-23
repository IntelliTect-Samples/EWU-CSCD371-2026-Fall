namespace PeopleEnricher.Tests;

internal class StubPeopleEndpointClient(
    Func<Task<string>>? downloadPeopleCsv = null,
    Func<string, Task<string>>? zipCodeLookup = null)
    : IPeopleEndpointClient
{
    public Task<string> DownloadPeopleCsvAsync(CancellationToken cancellationToken = default)
    {
        return downloadPeopleCsv?.Invoke() 
               ?? throw new InvalidOperationException(
                   "DownloadPeopleCsv behaviour not configured.");
    }

    public Task<string> ZipCodeLookupAsync(string zipCode, CancellationToken cancellationToken = default)
    {
        return zipCodeLookup?.Invoke(zipCode)
            ?? throw new InvalidOperationException(
                "ZipCodeLookup behaviour not configured.");
    }
}