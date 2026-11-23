using System.Text;

namespace PeopleEnricher;

public class PeopleEnricher(IPeopleEndpointClient peopleEndpointClient, TimeSpan? endpointTimeout = null)
{
    private readonly TimeSpan _endpointTimeout = endpointTimeout ?? TimeSpan.FromSeconds(5);

    private const string EnrichedHeader = "FirstName,LastName,EmailAddress,ZipCode,City,State";
    private const string ExpectedHeader = "FirstName,LastName,EmailAddress,ZipCode";
    
    private readonly Dictionary<string, Task<ZipLookupResult>> _zipLookupCache = [];
    
    public async Task<EnrichmentResult> EnrichAsync(CancellationToken cancellationToken = default)
    {
        string csv = await DownloadPeopleCsvAsync(cancellationToken);
        
        using StringReader stringReader = new(csv);
        
        string? header = await stringReader.ReadLineAsync(cancellationToken);
        
        ValidateHeader(header);
        
        List<RejectedRow> rejectedRows = [];
        List<Task<RowEnrichmentResult>> lookupTasks = [];

        int originalLineNumber = 1;

        while (await stringReader.ReadLineAsync(cancellationToken) is { } dataRow)
        {
            originalLineNumber++;
            
            string[] parts = dataRow.Split(',');

            if (!IsValidRow(parts))
            {
                rejectedRows.Add(new RejectedRow(
                    LineNumber: originalLineNumber,
                    RawData: dataRow,
                    Reason: "Invalid Data"));
                continue;
            }
            
            lookupTasks.Add(EnrichDataRowAsync(
                lineNumber: originalLineNumber,
                rawData: dataRow,
                zipCode: parts[3],
                cancellationToken));
        }
        
        RowEnrichmentResult[] lookupResults = await Task.WhenAll(lookupTasks);
        
        StringBuilder csvBuilder = new();
        
        csvBuilder.AppendLine(EnrichedHeader);

        foreach (RowEnrichmentResult result in lookupResults)
        {
            if (result.RejectedRow is not null)
            {
                rejectedRows.Add(result.RejectedRow);
                continue;
            }

            csvBuilder.AppendLine(
                result.EnrichedRow);
        }

        return new EnrichmentResult(
            EnrichedCsv: csvBuilder.ToString(),
            RejectedRows: rejectedRows);
    }

    private async Task<string> DownloadPeopleCsvAsync(CancellationToken cancellationToken)
    {
        try
        {
            return await peopleEndpointClient.DownloadPeopleCsvAsync(cancellationToken)
                .WaitAsync(_endpointTimeout, cancellationToken);
        }
        catch (HttpRequestException exception)
        {
            throw new EnrichmentException(
                errorType: EnrichmentErrorType.CsvDownloadFailed,
                "Error in connection with endpoint occured.",
                exception);
        }
        catch (TimeoutException exception)
        {
            throw new EnrichmentException(
                errorType: EnrichmentErrorType.CsvDownloadTimeout,
                "Endpoint Timeout.",
                exception);
        }
    }

    private async Task<RowEnrichmentResult> EnrichDataRowAsync(
        int lineNumber,
        string rawData,
        string zipCode,
        CancellationToken cancellationToken)
    {
        ZipLookupResult lookupResult = 
            await GetOrAddZipLookupTask(zipCode, cancellationToken);

        if (lookupResult.FailureReason is not null)
        {
            return new RowEnrichmentResult(
                EnrichedRow: null,
                RejectedRow: new RejectedRow(
                    LineNumber: lineNumber,
                    RawData: rawData,
                    Reason: lookupResult.FailureReason));
        }

        return new RowEnrichmentResult(
            EnrichedRow: $"{rawData},{lookupResult.Location}",
            RejectedRow: null);
    }

    private Task<ZipLookupResult> GetOrAddZipLookupTask(string zipCode, CancellationToken cancellationToken)
    {
        if (_zipLookupCache.TryGetValue(zipCode, out Task<ZipLookupResult>? lookupTask))
        {
            return lookupTask;
        }
        
        lookupTask = PerformZipLookupAsync(zipCode, cancellationToken);
        _zipLookupCache[zipCode] = lookupTask;
        return lookupTask;
    }

    private async Task<ZipLookupResult> PerformZipLookupAsync(string zipCode, CancellationToken cancellationToken)
    {
        try
        {
            string rawLocation =
                await peopleEndpointClient.ZipCodeLookupAsync(zipCode, cancellationToken)
                    .WaitAsync(_endpointTimeout, cancellationToken);

            string formattedLocation = FormatLocation(rawLocation);

            return new ZipLookupResult(
                Location: formattedLocation,
                FailureReason: null);
        }
        catch (HttpRequestException)
        {
            return new ZipLookupResult(
                Location: null,
                FailureReason: "Zip Lookup Endpoint failed.");
        }
        catch (TimeoutException)
        {
            return new ZipLookupResult(
                Location: null,
                FailureReason: "Zip Lookup Endpoint Timeout.");
        }
        catch (InvalidDataException)
        {
            return new ZipLookupResult(
                Location: null,
                FailureReason: "Zip Lookup Endpoint returned Data deemed as Invalid.");
        }
    }

    private void ValidateHeader(string? header)
    {
        if (header != ExpectedHeader)
        {
            throw new EnrichmentException(
                errorType: EnrichmentErrorType.InvalidHeader,
                $"Csv header error, expected: '{ExpectedHeader}', received: '{header}'.");
        }
    }

    private string FormatLocation(string rawLocation)
    {
        string[] parts = rawLocation.Split(',');

        if (parts.Length != 2)
        {
            throw new InvalidDataException();
        }
        
        string city = parts[0].Trim('"');
        string state = parts[1].Trim('"');
        
        city = string.Join(' ',
            city.Split(' ', 
                StringSplitOptions.RemoveEmptyEntries)
            .Select(word => 
                char.ToUpperInvariant(word[0]) + 
                word[1..].ToLowerInvariant()));
        
        return $"{city},{state}";
    }

    private static bool IsValidRow(string[] parts)
    {
        return parts.Length == 4
            && IsValidString(parts[0])
            && IsValidString(parts[1])
            && IsValidString(parts[2])
            && IsValidZipCode(parts[3]);
    }

    private static bool IsValidString(string value) => 
        !string.IsNullOrWhiteSpace(value);

    private static bool IsValidZipCode(string zipCode) => 
        zipCode.Length == 5 && zipCode.All(char.IsDigit);
}