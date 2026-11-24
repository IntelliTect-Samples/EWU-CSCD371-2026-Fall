using System.Net;

namespace PeopleEnricher;

public class PeopleEnricher
{
    private readonly IPeopleEndpointClient _peopleEndpointClient;
    private readonly TimeSpan _endpointTimeout;
    private readonly int _maxConcurrency;
    private readonly int _maxAttempts;

    private const string EnrichedHeader = "FirstName,LastName,EmailAddress,ZipCode,City,State";
    private const string ExpectedHeader = "FirstName,LastName,EmailAddress,ZipCode";

    public PeopleEnricher(IPeopleEndpointClient peopleEndpointClient, TimeSpan? endpointTimeout = null,
        int maxConcurrency = 4, int maxAttempts = 3)
    {
        ArgumentNullException.ThrowIfNull(peopleEndpointClient);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxConcurrency);
        ArgumentOutOfRangeException.ThrowIfNegativeOrZero(maxAttempts);
        _endpointTimeout = endpointTimeout ?? TimeSpan.FromSeconds(5);
        if (_endpointTimeout <= TimeSpan.Zero || _endpointTimeout.TotalMilliseconds > uint.MaxValue - 1)
        {
            throw new ArgumentOutOfRangeException(nameof(endpointTimeout));
        }

        _peopleEndpointClient = peopleEndpointClient;
        _maxConcurrency = maxConcurrency;
        _maxAttempts = maxAttempts;
    }

    public async Task<EnrichmentResult> EnrichAsync(CancellationToken cancellationToken = default)
    {
        string csv = await DownloadPeopleCsvAsync(cancellationToken);
        using StringReader stringReader = new(csv);
        string? header = await stringReader.ReadLineAsync(cancellationToken);
        ValidateHeader(header);

        List<RejectedRow> rejectedRows = [];
        List<ParsedRow> validRows = [];
        int originalLineNumber = 1;

        while (await stringReader.ReadLineAsync(cancellationToken) is { } dataRow)
        {
            originalLineNumber++;
            string[] parts = dataRow.Split(',');
            if (!IsValidRow(parts))
            {
                rejectedRows.Add(new RejectedRow(originalLineNumber, dataRow, "Invalid Data"));
                continue;
            }

            validRows.Add(new ParsedRow(originalLineNumber, dataRow,
                new Person(parts[0], parts[1], parts[2], parts[3])));
        }

        using SemaphoreSlim lookupSlots = new(_maxConcurrency);
        Dictionary<string, Task<ZipLookupResult>> zipLookupCache = [];
        List<Task<RowEnrichmentResult>> lookupTasks = [];
        foreach (ParsedRow row in validRows)
        {
            lookupTasks.Add(EnrichDataRowAsync(row, zipLookupCache, lookupSlots, cancellationToken));
        }

        RowEnrichmentResult[] lookupResults = await Task.WhenAll(lookupTasks);
        cancellationToken.ThrowIfCancellationRequested();

        rejectedRows.AddRange(lookupResults
            .Where(result => result.RejectedRow is not null)
            .Select(result => result.RejectedRow!));

        IEnumerable<string> enrichedLines = lookupResults
            .Where(result => result.EnrichedRow is not null)
            .Select(result => result.EnrichedRow!);

        return new EnrichmentResult(
            EnrichedCsv: string.Join(Environment.NewLine, enrichedLines.Prepend(EnrichedHeader))
                + Environment.NewLine,
            RejectedRows: rejectedRows.OrderBy(row => row.LineNumber).ToArray());
    }

    public async Task<EnrichmentResult> EnrichToFileAsync(string outputPath,
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        string fullPath = Path.GetFullPath(outputPath);
        EnrichmentResult result = await EnrichAsync(cancellationToken);
        string temporaryPath = Path.Combine(Path.GetDirectoryName(fullPath)!,
            $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            await File.WriteAllTextAsync(temporaryPath, result.EnrichedCsv, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();
            File.Move(temporaryPath, fullPath, overwrite: true);
            return result;
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    private async Task<string> DownloadPeopleCsvAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await RequestWithRetriesAsync(
                token => _peopleEndpointClient.DownloadPeopleCsvAsync(token), cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new EnrichmentException(EnrichmentErrorType.CsvDownloadFailed,
                "Error in connection with endpoint occured.", exception);
        }
        catch (TimeoutException exception)
        {
            throw new EnrichmentException(EnrichmentErrorType.CsvDownloadTimeout,
                "Endpoint Timeout.", exception);
        }
    }

    private async Task<RowEnrichmentResult> EnrichDataRowAsync(ParsedRow row,
        Dictionary<string, Task<ZipLookupResult>> zipLookupCache, SemaphoreSlim lookupSlots,
        CancellationToken cancellationToken)
    {
        if (!zipLookupCache.TryGetValue(row.Person.ZipCode, out Task<ZipLookupResult>? lookupTask))
        {
            lookupTask = PerformZipLookupAsync(row.Person.ZipCode, lookupSlots, cancellationToken);
            zipLookupCache[row.Person.ZipCode] = lookupTask;
        }

        ZipLookupResult lookupResult = await lookupTask;
        return lookupResult.FailureReason is not null
            ? new RowEnrichmentResult(null, new RejectedRow(
                row.LineNumber, row.RawData, lookupResult.FailureReason))
            : new RowEnrichmentResult($"{row.RawData},{lookupResult.Location}", null);
    }

    private async Task<ZipLookupResult> PerformZipLookupAsync(string zipCode,
        SemaphoreSlim lookupSlots, CancellationToken cancellationToken)
    {
        await lookupSlots.WaitAsync(cancellationToken);
        try
        {
            string rawLocation = await RequestWithRetriesAsync(
                token => _peopleEndpointClient.ZipCodeLookupAsync(zipCode, token), cancellationToken);
            return new ZipLookupResult(FormatLocation(rawLocation), null);
        }
        catch (HttpRequestException)
        {
            return new ZipLookupResult(null, "Zip Lookup Endpoint failed.");
        }
        catch (TimeoutException)
        {
            return new ZipLookupResult(null, "Zip Lookup Endpoint Timeout.");
        }
        catch (InvalidDataException)
        {
            return new ZipLookupResult(null, "Zip Lookup Endpoint returned Data deemed as Invalid.");
        }
        finally
        {
            lookupSlots.Release();
        }
    }

    private async Task<string> RequestWithRetriesAsync(
        Func<CancellationToken, Task<string>> request, CancellationToken cancellationToken)
    {
        for (int attempt = 1; ; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            using CancellationTokenSource requestCancellation =
                CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            requestCancellation.CancelAfter(_endpointTimeout);
            try
            {
                return await request(requestCancellation.Token);
            }
            catch (OperationCanceledException exception) when (!cancellationToken.IsCancellationRequested)
            {
                if (attempt == _maxAttempts)
                {
                    throw new TimeoutException("Endpoint Timeout.", exception);
                }
            }
            catch (TimeoutException) when (attempt < _maxAttempts)
            {
            }
            catch (HttpRequestException exception) when (
                attempt < _maxAttempts && exception.StatusCode != HttpStatusCode.NotFound)
            {
            }
        }
    }

    private static void ValidateHeader(string? header)
    {
        if (header != ExpectedHeader)
        {
            throw new EnrichmentException(EnrichmentErrorType.InvalidHeader,
                $"Csv header error, expected: '{ExpectedHeader}', received: '{header}'.");
        }
    }

    private static string FormatLocation(string rawLocation)
    {
        string[] parts = rawLocation.Split(',');
        if (parts.Length != 2)
        {
            throw new InvalidDataException();
        }

        string city = Unquote(parts[0]);
        string state = Unquote(parts[1]);
        if (string.IsNullOrWhiteSpace(city) || city != city.Trim() ||
            city.Any(char.IsControl) || city.Contains('"') ||
            state.Length != 2 || !state.All(character => character is >= 'A' and <= 'Z'))
        {
            throw new InvalidDataException();
        }

        city = string.Join(' ', city.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpperInvariant(word[0]) + word[1..].ToLowerInvariant()));
        return $"{city},{state}";
    }

    private static string Unquote(string field)
    {
        if (field.StartsWith('"') || field.EndsWith('"'))
        {
            if (field.Length < 2 || field[0] != '"' || field[^1] != '"')
            {
                throw new InvalidDataException();
            }
            return field[1..^1];
        }
        return field;
    }

    private static bool IsValidRow(string[] parts) =>
        parts.Length == 4 &&
        parts.Take(3).All(value => !string.IsNullOrWhiteSpace(value) &&
            !value.Contains('"') && !value.Any(char.IsControl)) &&
        parts[3].Length == 5 && parts[3].All(character => character is >= '0' and <= '9');

    private sealed record ParsedRow(int LineNumber, string RawData, Person Person);
}
