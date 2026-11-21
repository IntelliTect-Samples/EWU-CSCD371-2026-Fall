namespace Assignment;

public sealed class HttpClientWrapper
{
    //TODO: Might make this more testable later on (use like IHttpClientFactory or have it injectable before it's initialized)
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
}
