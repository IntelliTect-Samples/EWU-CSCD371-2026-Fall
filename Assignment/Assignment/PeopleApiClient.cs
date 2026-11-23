using System.Net;
using Assignment.Generated;
using Microsoft.Kiota.Abstractions;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;

namespace Assignment;

public sealed class PeopleApiClient
{
    public const int MaxAttempts = 8;
    private const int BaseRetryDelayMilliseconds = 100;
    private const int MaxJitterMilliseconds = 100;
    private const string BaseUrl =
        "https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io/";
    private const string PeopleHeader = "FirstName,LastName,EmailAddress,ZipCode";
    private readonly MayhemMarmotClient generatedClient;

    public PeopleApiClient(HttpClient httpClient)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        HttpClientRequestAdapter adapter = new(
            new AnonymousAuthenticationProvider(),
            httpClient: httpClient);
        adapter.BaseUrl = BaseUrl.TrimEnd('/');
        generatedClient = new MayhemMarmotClient(adapter);
    }

    public Task<string> GetPeopleCsvAsync(string user, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(user);
        return GetWithRetryAsync(
            $"people/{Uri.EscapeDataString(user)}",
            async attemptCt =>
            {
                Stream? stream = await generatedClient.People[user].GetAsync(
                    cancellationToken: attemptCt);
                return stream is null ? null : await ReadStreamAsync(stream, attemptCt);
            },
            body => HasPeopleHeader(body) ? body : null,
            ct,
            requireRepeat: true);
    }

    public Task<Location> GetLocationAsync(string zipCode, CancellationToken ct = default)
    {
        ArgumentException.ThrowIfNullOrEmpty(zipCode);
        return GetWithRetryAsync(
            $"people/zip/{Uri.EscapeDataString(zipCode)}",
            async attemptCt =>
            {
                Stream? stream = await generatedClient.People.Zip[zipCode].GetAsync(
                    cancellationToken: attemptCt);
                return stream is null ? null : await ReadStreamAsync(stream, attemptCt);
            },
            CsvParser.ParseLocation,
            ct);
    }

    // Tries up to MaxAttempts times. Bad data, server errors and timeouts are retried.
    // People CSVs must come back identical twice, because "invalid data" can swap ZIPs.
    private static async Task<T> GetWithRetryAsync<T>(string path,
        Func<CancellationToken, Task<string?>> download,
        Func<string, T?> parse,
        CancellationToken ct, bool requireRepeat = false) where T : class
    {
        var seen = new HashSet<T>();
        Exception? lastError = null;

        for (int attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            ct.ThrowIfCancellationRequested();
            try
            {
                string? body = await download(ct);
                T? value = body is null ? null : parse(body);
                if (value is not null && (!requireRepeat || !seen.Add(value)))
                    return value;
            }
            catch (HttpRequestException ex) when (IsRetryable(ex.StatusCode))
            {
                lastError = ex;
            }
            catch (ApiException ex) when (IsRetryable((HttpStatusCode)ex.ResponseStatusCode))
            {
                lastError = ex;
            }
            catch (ApiException ex)
            {
                throw new HttpRequestException(
                    ex.Message,
                    ex,
                    (HttpStatusCode)ex.ResponseStatusCode);
            }
            catch (HttpRequestException ex)
            {
                throw new HttpRequestException(ex.Message, ex);
            }
            catch (TaskCanceledException ex) when (!ct.IsCancellationRequested)
            {
                lastError = ex;   // timeout, not the caller cancelling
            }
            catch (HttpIOException ex)
            {
                lastError = ex;
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

    private static async Task<string> ReadStreamAsync(Stream stream, CancellationToken ct)
    {
        using (stream)
        {
            using StreamReader reader = new(stream);
            return await reader.ReadToEndAsync(ct);
        }
    }

    private static bool HasPeopleHeader(string body)
    {
        int lineEnd = body.IndexOfAny(['\r', '\n']);
        return lineEnd >= 0
            && body.AsSpan(0, lineEnd).SequenceEqual(PeopleHeader.AsSpan());
    }

    internal static int GetRetryDelayMilliseconds(int attempt, int jitterMilliseconds) =>
        BaseRetryDelayMilliseconds * (1 << (attempt - 1)) + jitterMilliseconds;

    private static bool IsRetryable(HttpStatusCode? code) =>
        code is null or HttpStatusCode.RequestTimeout or HttpStatusCode.TooManyRequests ||
        (int?)code == 418 || (int?)code >= 500;
}
