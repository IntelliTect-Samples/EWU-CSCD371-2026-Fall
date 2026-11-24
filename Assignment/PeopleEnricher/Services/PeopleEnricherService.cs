using Microsoft.VisualBasic.FileIO;
using PeopleEnricher.Models;
using System.Net.Mail;
using System.Net;

namespace PeopleEnricher.Services;

public sealed class PeopleEnricherService
{
    private readonly HttpClient _httpClient;

    private const string BaseUrl =
        "https://mayhem-marmot-api.livelystone-63fe914c.westus.azurecontainerapps.io";

    public PeopleEnricherService(HttpClient httpClient)
    {
        _httpClient = httpClient
            ?? throw new ArgumentNullException(nameof(httpClient));
    }

    public async Task<string> DownloadPeopleCsvAsync(
     string user,
     CancellationToken cancellationToken,
     int? expectedPeopleCount = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(user);

        if (expectedPeopleCount is <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(expectedPeopleCount));
        }

        string url =
            $"{BaseUrl}/people/{Uri.EscapeDataString(user)}";

        return await GetWithRetryAsync(
            url,
            csv =>
            {
                if (string.IsNullOrWhiteSpace(csv))
                {
                    throw new FormatException(
                        "People CSV response is empty.");
                }

                var parsed = ParsePeopleCsv(csv);

                int totalPeople =
                    parsed.People.Count +
                    parsed.RejectedPeople.Count;

                if (totalPeople == 0)
                {
                    throw new FormatException(
                        "People CSV contains no data rows.");
                }

                if (expectedPeopleCount.HasValue &&
                    totalPeople != expectedPeopleCount.Value)
                {
                    throw new FormatException(
                        $"Expected {expectedPeopleCount.Value} people, " +
                        $"but received {totalPeople}.");
                }

                return csv;
            },
            cancellationToken);
    }

    // The CSV must have the expected header and four columns
    // Empty or malformed rows are rejected, and their row numbers are recorded
    public static (
        List<(int RowNumber, Person Person)> People,
        List<RejectedPerson> RejectedPeople)
        ParsePeopleCsv(string csv)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(csv);

        List<(int RowNumber, Person Person)> people = [];
        List<RejectedPerson> rejectedPeople = [];

        using StringReader reader = new(csv);

        string? header = reader.ReadLine()?.TrimStart('\uFEFF');

        if (header?.Trim() != "FirstName,LastName,EmailAddress,ZipCode")
        {
            throw new FormatException("Invalid people CSV header.");
        }

        int rowNumber = 1;
        string? line;

        while ((line = reader.ReadLine()) is not null)
        {
            rowNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                rejectedPeople.Add(
                    new RejectedPerson(rowNumber, "Empty row"));
                continue;
            }

            string[]? fields;

            try
            {
                using StringReader lineReader = new(line);
                using TextFieldParser parser = new(lineReader)
                {
                    HasFieldsEnclosedInQuotes = true
                };

                parser.SetDelimiters(",");
                fields = parser.ReadFields();
            }
            catch (MalformedLineException)
            {
                rejectedPeople.Add(
                    new RejectedPerson(rowNumber, "Malformed CSV row"));
                continue;
            }

            if (fields is null || fields.Length != 4)
            {
                rejectedPeople.Add(
                    new RejectedPerson(rowNumber, "Incorrect column count"));
                continue;
            }

            people.Add((
                rowNumber,
                new Person(
                    fields[0].Trim(),
                    fields[1].Trim(),
                    fields[2].Trim(),
                    fields[3].Trim()
                )
            ));
        }

        return (people, rejectedPeople);
    }

    // A person is invalid if either name is missing,
    // the email format is invalid, or the ZIP code is not exactly five digits

    public static string? GetPersonValidationError(Person person)
    {
        ArgumentNullException.ThrowIfNull(person);

        if (string.IsNullOrWhiteSpace(person.FirstName))
        {
            return "Missing first name";
        }

        if (string.IsNullOrWhiteSpace(person.LastName))
        {
            return "Missing last name";
        }

        if (string.IsNullOrWhiteSpace(person.EmailAddress) ||
            !MailAddress.TryCreate(person.EmailAddress, out MailAddress? email) ||
            !string.Equals(email.Address, person.EmailAddress,
                StringComparison.OrdinalIgnoreCase))
        {
            return "Invalid email address";
        }

        if (person.ZipCode.Length != 5 ||
            !person.ZipCode.All(c => c is >= '0' and <= '9'))
        {
            return "Invalid ZIP code";
        }

        return null;
    }

    public static (List<(int RowNumber, Person Person)> ValidPeople, List<RejectedPerson> RejectedPeople)
    FilterValidPeople(
        IEnumerable<(int RowNumber, Person Person)> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        var checkedPeople = people
            .Select(item => new
            {
                item.RowNumber,
                item.Person,
                Error = GetPersonValidationError(item.Person)
            })
            .ToList();

        var validPeople = checkedPeople
            .Where(item => item.Error is null)
            .Select(item => (item.RowNumber, item.Person))
            .ToList();

        var rejectedPeople = checkedPeople
            .Where(item => item.Error is not null)
            .Select(item => new RejectedPerson(
                item.RowNumber,
                item.Error!))
            .ToList();

        return (validPeople, rejectedPeople);
    }
    public async Task<Location> LookupLocationAsync(
    string zipCode,
    CancellationToken cancellationToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(zipCode);

        string url =
            $"{BaseUrl}/people/zip/{Uri.EscapeDataString(zipCode)}";

        return await GetWithRetryAsync(
            url, ParseLocationCsv, cancellationToken);
    }


    // Location data must contain a nonempty city and a two-letter uppercase state abbreviation
    // Invalid location responses can be retried
    private static Location ParseLocationCsv(string csv)
    {
        using StringReader reader = new(csv);
        using TextFieldParser parser = new(reader);

        parser.SetDelimiters(",");
        parser.HasFieldsEnclosedInQuotes = true;

        string[]? fields;

        try
        {
            fields = parser.ReadFields();
        }
        catch (MalformedLineException ex)
        {
            throw new FormatException("Invalid location CSV.", ex);
        }

        if (fields is null ||
            fields.Length != 2 ||
            string.IsNullOrWhiteSpace(fields[0]) ||
            fields[1].Length != 2 ||
            !fields[1].All(c => c is >= 'A' and <= 'Z') ||
            !parser.EndOfData)
        {
            throw new FormatException("Invalid city or state.");
        }

        return new Location(fields[0], fields[1]);
    }

    private async Task<T> GetWithRetryAsync<T>(
        string url,
        Func<string, T> parseResponse,
        CancellationToken cancellationToken)
    {
        const int maxAttempts = 5;

        for (int attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                using HttpResponseMessage response =
                    await _httpClient.GetAsync(url, cancellationToken);

                response.EnsureSuccessStatusCode();

                string text = await response.Content.ReadAsStringAsync(
                    cancellationToken);

                return parseResponse(text);
            }
            catch (HttpRequestException ex) when (
                attempt < maxAttempts && CanRetry(ex))
            {
                // Retry temporary HTTP failures.
            }
            catch (OperationCanceledException) when (
                !cancellationToken.IsCancellationRequested &&
                attempt < maxAttempts)
            {
                // Retry request timeouts.
            }
            catch (FormatException) when (attempt < maxAttempts)
            {
                // Retry invalid API response data.
            }

            int delay = 200 * (1 << (attempt - 1));

            await Task.Delay(delay, cancellationToken);
        }

        throw new InvalidOperationException(
            "The retry loop ended unexpectedly.");
    }

    private static bool CanRetry(HttpRequestException ex)
    {
        return ex.StatusCode is null ||
            ex.StatusCode == HttpStatusCode.RequestTimeout ||
            ex.StatusCode == HttpStatusCode.TooManyRequests ||
            ex.StatusCode == (HttpStatusCode)418 ||
            (int)ex.StatusCode >= 500;
    }

    public async Task<(
    PersonWithLocation? CompletedPerson,
    RejectedPerson? Rejection)> ProcessPersonAsync(
        int rowNumber,
        Person person,
        CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(person);
        cancellationToken.ThrowIfCancellationRequested();

        try
        {
            Location location = await LookupLocationAsync(
                person.ZipCode,
                cancellationToken);

            PersonWithLocation completedPerson = new(
                person.FirstName,
                person.LastName,
                person.EmailAddress,
                person.ZipCode,
                location.City,
                location.State);

            return (completedPerson, null);
        }
        catch (HttpRequestException ex)
        {
            return (null, new RejectedPerson(
                rowNumber, $"Location lookup failed: {ex.Message}"));
        }
        catch (FormatException ex)
        {
            return (null, new RejectedPerson(
                rowNumber, $"Invalid location data: {ex.Message}"));
        }
        catch (OperationCanceledException) when (
            !cancellationToken.IsCancellationRequested)
        {
            return (null, new RejectedPerson(
                rowNumber, "Location lookup timed out"));
        }
    }

    public async Task<ProcessingResult> ProcessPeopleAsync(
    IReadOnlyList<(int RowNumber, Person Person)> validPeople,
    CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(validPeople);

        using SemaphoreSlim semaphore = new(5);

        List<Task<(
            PersonWithLocation? CompletedPerson,
            RejectedPerson? Rejection)>> tasks = [];

        foreach (var (rowNumber, person) in validPeople)
        {
            tasks.Add(ProcessWithLimitAsync(rowNumber, person));
        }

        var results = await Task.WhenAll(tasks);

        var completedPeople = results
            .Where(result => result.CompletedPerson is not null)
            .Select(result => result.CompletedPerson!)
            .ToList();

        var rejectedPeople = results
            .Where(result => result.Rejection is not null)
            .Select(result => result.Rejection!)
            .ToList();

        return new ProcessingResult(completedPeople, rejectedPeople);

        async Task<(
            PersonWithLocation? CompletedPerson,
            RejectedPerson? Rejection)> ProcessWithLimitAsync(
                int rowNumber,
                Person person)
        {
            await semaphore.WaitAsync(cancellationToken);

            try
            {
                return await ProcessPersonAsync(
                    rowNumber,
                    person,
                    cancellationToken);
            }
            finally
            {
                semaphore.Release();
            }
        }
    }

    public async Task<ProcessingResult> RunAsync(
    string user,
    CancellationToken cancellationToken,
    int expectedPeopleCount = 100)
    {
        cancellationToken.ThrowIfCancellationRequested();

        // Download the original people CSV.
        string csv = await DownloadPeopleCsvAsync(
     user, cancellationToken, expectedPeopleCount);

        // Parse the CSV into Person records.
        var parsed = ParsePeopleCsv(csv);

        // Separate valid and invalid people.
        var filtered = FilterValidPeople(parsed.People);

        // Look up locations for valid people.
        ProcessingResult processed = await ProcessPeopleAsync(
            filtered.ValidPeople, cancellationToken);

        // Combine all rejected records.
        List<RejectedPerson> rejectedPeople = parsed.RejectedPeople
            .Concat(filtered.RejectedPeople)
            .Concat(processed.RejectedPeople)
            .OrderBy(person => person.RowNumber)
            .ToList();

        return new ProcessingResult(
            processed.CompletedPeople,
            rejectedPeople);
    }

    public static string CreateCsv(ProcessingResult result)
    {
        ArgumentNullException.ThrowIfNull(result);

        const string header =
            "FirstName,LastName,EmailAddress,ZipCode,City,State";

        var rows = result.CompletedPeople
            .Select(person => string.Join(",",
                EscapeCsvField(person.FirstName),
                EscapeCsvField(person.LastName),
                EscapeCsvField(person.EmailAddress),
                EscapeCsvField(person.ZipCode),
                EscapeCsvField(person.City),
                EscapeCsvField(person.State)));

        return string.Join(
            Environment.NewLine,
            rows.Prepend(header)) + Environment.NewLine;
    }

    private static string EscapeCsvField(string value)
    {
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }

    public static async Task SaveCsvAsync(
    ProcessingResult result,
    string outputPath,
    CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(result);
        ArgumentException.ThrowIfNullOrWhiteSpace(outputPath);

        string fullPath = Path.GetFullPath(outputPath);
        string directory = Path.GetDirectoryName(fullPath)!;

        string tempPath = Path.Combine(
            directory,
            $".{Path.GetFileName(fullPath)}.{Guid.NewGuid():N}.tmp");

        try
        {
            string csv = CreateCsv(result);

            await File.WriteAllTextAsync(
                tempPath, csv, cancellationToken);

            cancellationToken.ThrowIfCancellationRequested();

            File.Move(tempPath, fullPath, overwrite: true);
        }
        finally
        {
            if (File.Exists(tempPath))
            {
                File.Delete(tempPath);
            }
        }
    }
}