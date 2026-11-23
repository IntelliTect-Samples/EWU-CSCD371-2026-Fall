namespace PeopleEnricher;

public class PeopleEndpointClient : IPeopleEndpointClient
{
    private static readonly HttpClient HttpClient = new();

    static PeopleEndpointClient()
    {
        HttpClient.BaseAddress = new Uri(
            "https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io");
    }

    public async Task<string> DownloadPeopleCsvAsync(CancellationToken cancellationToken = default)
    {
        return await HttpClient.GetStringAsync("/people/WillAndNico", cancellationToken);
    }

    public async Task<string> ZipCodeLookupAsync(string zipCode, CancellationToken cancellationToken = default)
    {
        return await HttpClient.GetStringAsync($"/people/zip/{zipCode}", cancellationToken);
    }
}