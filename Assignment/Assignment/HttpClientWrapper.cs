namespace Assignment;

public sealed class HttpClientWrapper
{
    private static readonly Lazy<HttpClient> _httpClient = new(
        () =>
        {
            var handler = new SocketsHttpHandler
            {
                PooledConnectionLifetime = TimeSpan.FromMinutes(2)
            };

            return new HttpClient(handler, true)
            {
                BaseAddress = new Uri("https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io")
            };
        });

    public static HttpClient Client => _httpClient.Value;

    public static async Task<string> DownloadCsvAsync(CancellationToken cancellationToken = default)
    {
        string rows = await Client.GetStringAsync("/people/user", cancellationToken);

        cancellationToken.ThrowIfCancellationRequested();

        return rows;
    }
}
