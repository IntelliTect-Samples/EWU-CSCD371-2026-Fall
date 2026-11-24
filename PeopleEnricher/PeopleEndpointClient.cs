namespace PeopleEnricher;

/// <summary>Uses a shared client by default; injected clients remain caller-owned.</summary>
public class PeopleEndpointClient : IPeopleEndpointClient
{
    private static readonly HttpClient SharedHttpClient = new();
    private readonly HttpClient _httpClient;
    private const string BaseUrl =
        "https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io";

    public PeopleEndpointClient() : this(SharedHttpClient)
    {
    }

    public PeopleEndpointClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        _httpClient = httpClient;
    }

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
