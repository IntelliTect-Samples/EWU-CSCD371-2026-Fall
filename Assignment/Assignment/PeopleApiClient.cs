using System.Net;

namespace Assignment;

public sealed class PeopleApiClient(HttpClient httpClient)
{
    public const int MaxAttempts = 8;
    private const int BaseRetryDelayMilliseconds = 100;
    private const int MaxJitterMilliseconds = 100;
    private const string BaseUrl =
        "https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io/";
    private const string PeopleHeader = "FirstName,LastName,EmailAddress,ZipCode";

    public Task<string> GetPeopleCsvAsync(string user, CancellationToken ct = default) =>
        GetWithRetryAsync("people/" + Uri.EscapeDataString(user),
            body => body.StartsWith(PeopleHeader, StringComparison.Ordinal) ? body : null,
            ct, requireRepeat: true);

    public Task<Location> GetLocationAsync(string zipCode, CancellationToken ct = default) =>
        GetWithRetryAsync("people/zip/" + Uri.EscapeDataString(zipCode), CsvParser.ParseLocation, ct);

    // Tries up to MaxAttempts times. Bad data, server errors and timeouts are retried.
    // People CSVs must come back identical twice, because "invalid data" can swap ZIPs.
    private async Task<T> GetWithRetryAsync<T>(string path, Func<string, T?> parse,
        CancellationToken ct, bool requireRepeat = false) where T : class
    {
        var seen = new HashSet<T>();
        Exception? lastError = null;

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                T? value = parse(await httpClient.GetStringAsync(BaseUrl + path, ct));
                if (value is not null && (!requireRepeat || !seen.Add(value)))
                    return value;
            }
            catch (HttpRequestException ex) when (IsRetryable(ex.StatusCode))
            {
                lastError = ex;
            }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
            {
                lastError = ex;   // timeout, not the caller cancelling
            }

            if (attempt < MaxAttempts)
            {
                // Increase the wait each retry and add jitter to spread concurrent requests.
                int jitterMilliseconds = Random.Shared.Next(MaxJitterMilliseconds);
                await Task.Delay(GetRetryDelayMilliseconds(attempt, jitterMilliseconds), ct);
            }
        }

        throw new HttpRequestException($"GET {path} failed after {MaxAttempts} attempts.", lastError);
    }

    internal static int GetRetryDelayMilliseconds(int attempt, int jitterMilliseconds) =>
        BaseRetryDelayMilliseconds * (1 << (attempt - 1)) + jitterMilliseconds;

    private static bool IsRetryable(HttpStatusCode? code) =>
        code is null or HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests ||
        (int?)code == 418 || (int?)code >= 500;
}
