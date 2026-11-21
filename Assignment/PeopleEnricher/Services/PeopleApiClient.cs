namespace PeopleEnricher;

/// <summary>
/// Retrieves and validates people and ZIP location data from the API.
/// </summary>
/// <remarks>
/// The caller owns the supplied HttpClient and is responsible for disposing it.
/// This client reuses that instance for all requests.
/// </remarks>
public sealed class PeopleApiClient : IPeopleApiClient
{
    private static readonly Uri ApiBaseUri = new(
        "https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io/");

    private readonly HttpClient _httpClient;
    private readonly int _maxAttempts;
    private readonly TimeSpan _requestTimeout;
    private readonly TimeSpan _retryDelay;

    /// <summary>
    /// Creates an API client using a caller-supplied HttpClient.
    /// </summary>
    /// <param name="httpClient">The reusable HTTP client.</param>
    /// <param name="maxAttempts">The maximum attempts per request.</param>
    /// <param name="requestTimeout">The timeout for each request.</param>
    /// <param name="retryDelay">The delay between attempts.</param>
    public PeopleApiClient(
        HttpClient httpClient,
        int maxAttempts = 3,
        TimeSpan? requestTimeout = null,
        TimeSpan? retryDelay = null)
    {
        ArgumentNullException.ThrowIfNull(httpClient);

        if (maxAttempts < 1)
        {
            throw new ArgumentOutOfRangeException(
                nameof(maxAttempts),
                "At least one attempt is required.");
        }

        TimeSpan timeout = requestTimeout ?? TimeSpan.FromSeconds(10);
        TimeSpan delay = retryDelay ?? TimeSpan.FromMilliseconds(200);

        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(requestTimeout),
                "The request timeout must be positive.");
        }

        if (delay < TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(
                nameof(retryDelay),
                "The retry delay cannot be negative.");
        }

        _httpClient = httpClient;
        _maxAttempts = maxAttempts;
        _requestTimeout = timeout;
        _retryDelay = delay;
    }

    /// <summary>
    /// Downloads and parses the people CSV.
    /// </summary>
    /// <param name="user">The user name or identifier.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The valid and rejected people records.</returns>
    public Task<PeopleCsvParseResult> GetPeopleAsync(
        string user,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user);

        Uri requestUri = new(
            ApiBaseUri,
            $"people/{Uri.EscapeDataString(user)}");

        return GetAndParseAsync(
            requestUri,
            PeopleCsvParser.Parse,
            cancellationToken);
    }

    /// <summary>
    /// Retrieves and validates the city and state for a ZIP code.
    /// </summary>
    /// <param name="zipCode">The five-digit ZIP code.</param>
    /// <param name="cancellationToken">The cancellation token.</param>
    /// <returns>The validated city and state.</returns>
    public Task<ZipLocation> GetZipLocationAsync(
        string zipCode,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(zipCode);

        if (zipCode.Length != 5 ||
            zipCode.Any(character =>
                character < '0' || character > '9'))
        {
            throw new ArgumentException(
                "ZIP codes must contain exactly five numeric digits.",
                nameof(zipCode));
        }

        Uri requestUri = new(
            ApiBaseUri,
            $"people/zip/{Uri.EscapeDataString(zipCode)}");

        return GetAndParseAsync(
            requestUri,
            ZipLocationParser.Parse,
            cancellationToken);
    }

    private async Task<TResult> GetAndParseAsync<TResult>(
        Uri requestUri,
        Func<string, TResult> parser,
        CancellationToken cancellationToken)
    {
        for (int attempt = 1; attempt <= _maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                using CancellationTokenSource timeoutSource =
                    new(_requestTimeout);

                using CancellationTokenSource requestSource =
                    CancellationTokenSource.CreateLinkedTokenSource(
                        cancellationToken,
                        timeoutSource.Token);

                string response = await _httpClient
                    .GetStringAsync(requestUri, requestSource.Token)
                    .ConfigureAwait(false);

                return parser(response);
            }
            catch (OperationCanceledException)
                when (!cancellationToken.IsCancellationRequested &&
                      attempt < _maxAttempts)
            {
                await DelayBeforeRetryAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (HttpRequestException)
                when (attempt < _maxAttempts)
            {
                await DelayBeforeRetryAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (InvalidDataException)
                when (attempt < _maxAttempts)
            {
                await DelayBeforeRetryAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (IOException)
                when (attempt < _maxAttempts)
            {
                await DelayBeforeRetryAsync(cancellationToken)
                    .ConfigureAwait(false);
            }
        }

        throw new InvalidOperationException(
            "The request retry loop ended unexpectedly.");
    }

    private async Task DelayBeforeRetryAsync(
        CancellationToken cancellationToken)
    {
        if (_retryDelay > TimeSpan.Zero)
        {
            await Task.Delay(_retryDelay, cancellationToken)
                .ConfigureAwait(false);
        }
    }
}