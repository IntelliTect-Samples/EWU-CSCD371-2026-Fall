namespace Assignment;

public sealed class HttpClientWrapper(IHttpClientFactory httpClientFactory)
{
    public const string ClientName = "MayhemMarmotApi";

    // GET /people/{user} -> CSV of 100 people
    public Task<DownloadResult> DownloadPeopleCsvAsync(string user, CancellationToken cancellationToken = default)
        => GetAsync($"/people/{Uri.EscapeDataString(user)}", cancellationToken);

    // GET /people/zip/{zip} -> one line like "Spokane","WA"
    public Task<DownloadResult> DownloadZipAsync(string zip, CancellationToken cancellationToken = default)
        => GetAsync($"/people/zip/{Uri.EscapeDataString(zip)}", cancellationToken);

    private async Task<DownloadResult> GetAsync(string path, CancellationToken cancellationToken)
    {
        try
        {
            var client = httpClientFactory.CreateClient(ClientName);

            string body = await client.GetStringAsync(path, cancellationToken);

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
        catch (Exception ex)
        {
            return new DownloadResult(DownloadStatus.Exception, Error: ex);
        }
    }
}