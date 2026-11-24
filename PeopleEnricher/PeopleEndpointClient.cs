namespace PeopleEnricher;

/// <summary>Uses a caller-owned, reused HttpClient. The caller disposes it.</summary>
public class PeopleEndpointClient(HttpClient httpClient) : IPeopleEndpointClient
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private const string BaseUrl =
        "https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io";

    public async Task<string> DownloadPeopleCsvAsync(CancellationToken cancellationToken = default)
    {
        return await GetCsvAsync($"{BaseUrl}/people/WillAndNico", cancellationToken);
    }

    public async Task<string> ZipCodeLookupAsync(string zipCode, CancellationToken cancellationToken = default)
    {
        return await GetCsvAsync($"{BaseUrl}/people/zip/{zipCode}", cancellationToken);
    }

    private async Task<string> GetCsvAsync(string url, CancellationToken cancellationToken)
    {
        using HttpResponseMessage response = await _httpClient.GetAsync(
            url, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadAsStringAsync(cancellationToken);
    }
}
