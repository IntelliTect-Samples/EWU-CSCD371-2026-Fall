namespace PeopleEnricher.Tests;

internal sealed class StubPeopleEndpointClient(
    Func<Task<string>>? downloadPeopleCsv = null,
    Func<string, Task<string>>? zipCodeLookup = null,
    Func<CancellationToken, Task<string>>? downloadPeopleCsvWithCancellation = null,
    Func<string, CancellationToken, Task<string>>? zipCodeLookupWithCancellation = null)
    : IPeopleEndpointClient
{
    public Task<string> DownloadPeopleCsvAsync(CancellationToken cancellationToken = default)
    {
        return downloadPeopleCsvWithCancellation?.Invoke(cancellationToken)
               ?? downloadPeopleCsv?.Invoke()
               ?? throw new InvalidOperationException("DownloadPeopleCsv behaviour not configured.");
    }

    public Task<string> ZipCodeLookupAsync(string zipCode, CancellationToken cancellationToken = default)
    {
        return zipCodeLookupWithCancellation?.Invoke(zipCode, cancellationToken)
               ?? zipCodeLookup?.Invoke(zipCode)
               ?? throw new InvalidOperationException("ZipCodeLookup behaviour not configured.");
    }
}
