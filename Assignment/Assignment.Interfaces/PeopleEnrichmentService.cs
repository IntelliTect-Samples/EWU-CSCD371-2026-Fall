using System.Net;

namespace Assignment.Interfaces;

/// <summary>Downloads people data, enriches locations, and writes the results to a CSV file.</summary>
public sealed class PeopleEnrichmentService : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _requestLimiter;
    private readonly int _retryCount;
    private readonly TimeSpan _retryDelay;
    private readonly bool _disposeHttpClient;
    private bool _disposed;

    /// <summary>Creates a resilient people downloader and location enricher.</summary>
    /// <param name="httpClient">The HTTP client used to call the people API.</param>
    /// <param name="maxConcurrentRequests">The maximum number of concurrent requests.</param>
    /// <param name="retryCount">The maximum number of attempts for each request.</param>
    /// <param name="retryDelay">The delay between failed attempts.</param>
    /// <param name="disposeHttpClient">Whether disposing this service also disposes the HTTP client.</param>
    public PeopleEnrichmentService(
        HttpClient httpClient,
        int maxConcurrentRequests = 10,
        int retryCount = 5,
        TimeSpan? retryDelay = null,
        bool disposeHttpClient = false)
    {
        ArgumentNullException.ThrowIfNull(httpClient);
        ArgumentOutOfRangeException.ThrowIfLessThan(maxConcurrentRequests, 1);
        ArgumentOutOfRangeException.ThrowIfLessThan(retryCount, 1);

        _httpClient = httpClient;
        _requestLimiter = new SemaphoreSlim(maxConcurrentRequests, maxConcurrentRequests);
        _retryCount = retryCount;
        _retryDelay = retryDelay ?? TimeSpan.FromMilliseconds(100);
        _disposeHttpClient = disposeHttpClient;
    }

    /// <summary>
    /// Downloads, validates, concurrently enriches, and atomically writes the final CSV.
    /// </summary>
    /// <param name="user">The user whose people data should be downloaded.</param>
    /// <param name="outputPath">The path where the enriched CSV should be written.</param>
    /// <param name="cancellationToken">A token that can cancel the operation.</param>
    /// <returns>Counts and rejection details for the completed run.</returns>
    /// <exception cref="PeopleDownloadException">The people data could not be downloaded after retrying.</exception>
    public async Task<RunResult> RunAsync(string user, string outputPath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);
        ObjectDisposedException.ThrowIf(_disposed, this);

        var peopleCsv = await DownloadPeopleAsync(user, cancellationToken).ConfigureAwait(false);
        var parsedRows = ParsePeopleCsv(peopleCsv);
        var totalRows = parsedRows.Length;

        var rejections = parsedRows
            .Where(personAndRow => !Csv.IsValidPerson(personAndRow.Person))
            .Select(personAndRow => new Rejection(
                personAndRow.RowDescription,
                Csv.PersonInvalidReason(personAndRow.Person)))
            .ToList();

        var validPeople = parsedRows
            .Where(personAndRow => Csv.IsValidPerson(personAndRow.Person))
            .Select(personAndRow => personAndRow.Person)
            .ToArray();

        var enrichmentTasks = validPeople
            .Select(person => LookupLocationAsync(person, cancellationToken))
            .ToArray();
        var lookups = await Task.WhenAll(enrichmentTasks).ConfigureAwait(false);

        rejections.AddRange(lookups
            .Where(lookup => lookup.Person is null)
            .Select(lookup => new Rejection(
                $"{lookup.RejectionPerson!.FirstName} {lookup.RejectionPerson.LastName}",
                lookup.RejectionReason ?? "Location lookup failed")));

        var enrichedPeople = lookups
            .Where(lookup => lookup.Person is not null)
            .Select(lookup => lookup.Person!)
            .ToArray();

        await WriteOutputAtomicallyAsync(outputPath, enrichedPeople, cancellationToken).ConfigureAwait(false);

        return new RunResult(
            totalRows,
            rejections.Count,
            enrichedPeople.Length,
            rejections);
    }


    /// <summary>
    /// Downloads people from CSV as an async task 
    /// </summary>
    /// <param name="user"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    /// <exception cref="PeopleDownloadException"></exception>
    private async Task<string> DownloadPeopleAsync(string user, CancellationToken cancellationToken)
    {
        Exception? lastException = null;

        for (var attempt = 1; attempt <= _retryCount; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                using var request = new HttpRequestMessage(HttpMethod.Get, $"people/{Uri.EscapeDataString(user)}");
                using var response = await _httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken).ConfigureAwait(false);
                response.EnsureSuccessStatusCode();
                var csv = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);

                if (!IsValidPeopleHeader(csv))
                {
                    throw new InvalidDataException("The people CSV header is missing or invalid.");
                }

                ParsePeopleCsv(csv);
                return csv;
            }
            catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
            {
                throw;
            }
            catch (Exception exception) when (
                exception is HttpRequestException ||
                exception is TaskCanceledException ||
                exception is InvalidDataException)
            {
                lastException = exception;
            }

            if (attempt < _retryCount)
            {
                await DelayAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        throw new PeopleDownloadException(
            $"The people CSV could not be downloaded after {_retryCount} attempts.",
            lastException);
    }


    /// <summary>
    /// Header for ensuring a valid person target is selected
    /// </summary>
    /// <param name="csv"></param>
    /// <returns></returns>
    private static bool IsValidPeopleHeader(string csv)
    {
        var rows = Csv.Parse(csv);
        if (rows.Count == 0)
        {
            return false;
        }

        var header = rows[0];
        return header.Length == 4 &&
               header[0].TrimStart('\uFEFF').Equals("FirstName", StringComparison.OrdinalIgnoreCase) &&
               header[1].Equals("LastName", StringComparison.OrdinalIgnoreCase) &&
               header[2].Equals("EmailAddress", StringComparison.OrdinalIgnoreCase) &&
               header[3].Equals("ZipCode", StringComparison.OrdinalIgnoreCase);
    }


    /// <summary>
    /// This parses the CSV of people
    /// </summary>
    /// <param name="csv"></param>
    /// <returns></returns>
    /// <exception cref="InvalidDataException"></exception>
    private static (Person Person, string RowDescription)[] ParsePeopleCsv(string csv)
    {
        var rows = Csv.Parse(csv);
        if (rows.Count < 1)
        {
            throw new InvalidDataException("The people CSV is empty.");
        }

        return rows.Skip(1)
            .Select(row => row.Length == 4
                ? new Person(row[0].Trim(), row[1].Trim(), row[2].Trim(), row[3].Trim())
                : new Person(
                    row.ElementAtOrDefault(0)?.Trim() ?? string.Empty,
                    row.ElementAtOrDefault(1)?.Trim() ?? string.Empty,
                    string.Empty,
                    string.Empty))
            .Select((person, index) => (
                person,
                RowDescription: $"row {index + 2}: {person.FirstName} {person.LastName}".Trim()))
            .ToArray();
    }

    /// <summary>
    /// Location lookup of a person in a CSV as an async task 
    /// </summary>
    /// <param name="person"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task<LocationLookup> LookupLocationAsync(Person person, CancellationToken cancellationToken)
    {
        await _requestLimiter.WaitAsync(cancellationToken).ConfigureAwait(false);
        try
        {
            Exception? lastException = null;

            for (var attempt = 1; attempt <= _retryCount; attempt++)
            {
                cancellationToken.ThrowIfCancellationRequested();
                try
                {
                    using var request = new HttpRequestMessage(
                        HttpMethod.Get,
                        $"people/zip/{Uri.EscapeDataString(person.ZipCode)}");
                    using var response = await _httpClient.SendAsync(
                        request,
                        HttpCompletionOption.ResponseHeadersRead,
                        cancellationToken).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();
                    var csv = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
                    var rows = Csv.Parse(csv);
                    if (rows.Count == 1 && rows[0].Length == 2)
                    {
                        var city = rows[0][0].Trim();
                        var state = rows[0][1].Trim();
                        if (Csv.IsValidLocation(city, state))
                        {
                            return new LocationLookup(
                                new EnrichedPerson(
                                    person.FirstName,
                                    person.LastName,
                                    person.EmailAddress,
                                    person.ZipCode,
                                    city,
                                    state),
                                null,
                                null);
                        }
                    }

                    lastException = new InvalidDataException("Location endpoint returned invalid city or state data.");
                }
                catch (OperationCanceledException) when (cancellationToken.IsCancellationRequested)
                {
                    throw;
                }
                catch (Exception exception) when (
                    exception is HttpRequestException ||
                    exception is TaskCanceledException ||
                    exception is InvalidDataException)
                {
                    lastException = exception;
                }

                if (attempt < _retryCount)
                {
                    await DelayAsync(cancellationToken).ConfigureAwait(false);
                }
            }

            var reason = lastException is InvalidDataException
                ? "Invalid location data"
                : "Location lookup failed";
            return new LocationLookup(null, person, reason);
        }
        finally
        {
            _requestLimiter.Release();
        }
    }

    /// <summary>
    /// Delay Async task function to ensure memory doesnt hang
    /// </summary>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private async Task DelayAsync(CancellationToken cancellationToken)
    {
        if (_retryDelay > TimeSpan.Zero)
        {
            await Task.Delay(_retryDelay, cancellationToken).ConfigureAwait(false);
        }
    }

    /// <summary>
    /// Write structured output as an async task 
    /// </summary>
    /// <param name="outputPath"></param>
    /// <param name="people"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    private static async Task WriteOutputAtomicallyAsync(
        string outputPath,
        IReadOnlyCollection<EnrichedPerson> people,
        CancellationToken cancellationToken)
    {
        var fullPath = Path.GetFullPath(outputPath);
        var directory = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(directory))
        {
            Directory.CreateDirectory(directory);
        }

        var temporaryPath = fullPath + "." + Guid.NewGuid().ToString("N", null) + ".tmp";
        try
        {
            await using (var stream = new FileStream(
                temporaryPath,
                FileMode.CreateNew,
                FileAccess.Write,
                FileShare.None,
                4096,
                FileOptions.Asynchronous))
            await using (var writer = new StreamWriter(stream, new System.Text.UTF8Encoding(false)))
            {
                await writer.WriteLineAsync("FirstName,LastName,EmailAddress,ZipCode,City,State").ConfigureAwait(false);
                foreach (var person in people)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    var line = Csv.FormatRow([
                        person.FirstName,
                        person.LastName,
                        person.EmailAddress,
                        person.ZipCode,
                        person.City,
                        person.State]);
                    await writer.WriteLineAsync(line.AsMemory(), cancellationToken).ConfigureAwait(false);
                }

                await writer.FlushAsync(cancellationToken).ConfigureAwait(false);
            }

            cancellationToken.ThrowIfCancellationRequested();
            File.Move(temporaryPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(temporaryPath))
            {
                File.Delete(temporaryPath);
            }
        }
    }

    /// <summary>Releases the request limiter and, when configured, the HTTP client.</summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _requestLimiter.Dispose();
        if (_disposeHttpClient)
        {
            _httpClient.Dispose();
        }
        _disposed = true;
    }

    /// <summary>
    /// This record is in regards to the location lookup function. Look there for additional guidance 
    /// </summary>
    /// <param name="Person"></param>
    /// <param name="RejectionPerson"></param>
    /// <param name="RejectionReason"></param>
    private sealed record LocationLookup(
        EnrichedPerson? Person,
        Person? RejectionPerson,
        string? RejectionReason);
}
