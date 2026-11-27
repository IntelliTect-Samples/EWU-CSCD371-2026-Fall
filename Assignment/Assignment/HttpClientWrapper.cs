namespace Assignment;

public sealed class HttpClientWrapper(IHttpClientFactory httpClientFactory)
{
    private static readonly Random _jitterer = new();
    public const string ClientName = "MayhemMarmotApi";

    // GET /people/{user} -> CSV of 100 people
    public Task<DownloadResult> DownloadPeopleCsvAsync(string user, CancellationToken cancellationToken = default)
        => GetAsync($"/people/{Uri.EscapeDataString(user)}?code=None", cancellationToken);

    // GET /people/zip/{zip} -> one line like "Spokane","WA"
    public Task<DownloadResult> DownloadZipAsync(string zip, CancellationToken cancellationToken = default)
        => GetAsync($"/people/zip/{Uri.EscapeDataString(zip)}?code=None", cancellationToken);

    private async Task<DownloadResult> GetAsync(string path, CancellationToken cancellationToken)
    {
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
        linkedCts.CancelAfter(TimeSpan.FromSeconds(30)); // timeout
        var client = httpClientFactory.CreateClient(ClientName);
        const int maxAttempts = 10;
        for (int attempt = 1; attempt < maxAttempts; attempt++)
        {
            try
            {
                string body = await client.GetStringAsync(path, linkedCts.Token);

                return string.IsNullOrWhiteSpace(body)
                    ? new DownloadResult(DownloadStatus.InvalidData)
                    : new DownloadResult(DownloadStatus.Success, Csv: body);
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested) // cancellation requested
            {
                return new DownloadResult(DownloadStatus.Cancelled);
            }
            catch (OperationCanceledException ex) // timeout
            {
                return new DownloadResult(DownloadStatus.Timeout, Error: ex);
            }
            catch (HttpRequestException ex) when (attempt < maxAttempts - 1) // waits for an exponentially growing amount of time with jitter, then retries
            {
                try
                {
                    var delay = TimeSpan.FromSeconds(Math.Pow(2, attempt)) + TimeSpan.FromMicroseconds(_jitterer.Next(0, 1000));
                    await Task.Delay(TimeSpan.FromMilliseconds(5), cancellationToken);
                }
                catch (TaskCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    return new DownloadResult(DownloadStatus.Exception, Error: ex);
                }
            }
            catch (HttpRequestException)
            {
                return new DownloadResult(DownloadStatus.Exception, Error: ex);
            }
        }
    }
}