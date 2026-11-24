using System.Collections.Concurrent;

namespace Assignment;

public class PeopleEnricher
{
    internal sealed record EnrichedPerson(Person Person, string City, string State)
    {
        public override string ToString()
        {
            return $"{this.Person},{City},{State}";
        }
    }
    internal sealed record Result(Person Person, DownloadResult DownloadResult);
    private HttpClientWrapper _client;
    private TextWriter _writer;
    private ConcurrentBag<string> _errors = [];
    public PeopleEnricher(HttpClientWrapper client, TextWriter? writer = null)
    {
        ArgumentNullException.ThrowIfNull(client, nameof(client));
        _client = client;
        _writer = writer ?? Console.Out;
    }

    private static string? GetErrorMessage(DownloadResult result, string zip)
    {
        return result.Status switch
        {
            DownloadStatus.Cancelled => $"Error downloading data for zip {zip} - Operation cancelled",
            DownloadStatus.Exception => $"Error downloading data for zip {zip} - Exception caught {(result.Error is not null ? result.Error.Message : "")}",
            DownloadStatus.Timeout => $"Error downloading data for zip {zip} - Timeout occurred",
            _ => null,
        };
    }

    private async Task<Result> DownloadCityState(Person person, SemaphoreSlim limiter, CancellationToken cancellationToken)
    {
        await limiter.WaitAsync(cancellationToken);
        try
        {
            DownloadResult result = await _client.DownloadZipAsync(person.ZipCode, cancellationToken);
            return new Result(person, result);
        }
        finally
        {
            limiter.Release();
        }
    }

    private async Task<List<EnrichedPerson>> EnrichPeopleAsync(IEnumerable<Person> people, SemaphoreSlim limiter, CancellationToken cancellationToken)
    {
        Task<Result>[] tasks = [.. people.Select(person => DownloadCityState(person, limiter, cancellationToken))];
        List<EnrichedPerson> enrichedPeople = [];
        var results = await Task.WhenAll(tasks);

        if (results is null)
        {
            throw new ArgumentNullException(paramName: nameof(results));
        }

        else
        {

            foreach (var result in results)
            {
                if (result.DownloadResult is null || result.DownloadResult.Csv is null)
                {
                    _errors.Add("Result is null! Skipping row");
                    continue;
                }

                string? error = GetErrorMessage(result.DownloadResult!, result.Person.ZipCode);

                if (error is not null)
                {
                    _errors.Add(error);
                }
                else
                {
                    //validate
                    var csv = result.DownloadResult.Csv;
                    var tokens = csv.Split(',');
                    tokens.Trim("\"");
                    if (tokens == null || tokens.Length != 2)
                    {
                        _errors.Add($"Bad data: {csv}");
                    }
                    else
                    {
                        string city = tokens[0];
                        city = char.ToUpper(city[0]) + city[1..].ToLower();
                        string state = tokens[1];

                        enrichedPeople.Add(new EnrichedPerson(result.Person, city, state));
                    }
                }
            }

        }

        return enrichedPeople;
    }

    public async Task<string?> BuildEnrichedCsvAsync(IEnumerable<Person> people, CancellationToken cancellationToken)
    {
        var enriched = await EnrichPeopleAsync(people, new SemaphoreSlim(4), cancellationToken);
        if (enriched is null) return null;

        var result = enriched.Select(r => r.ToString()).Aggregate((a, b) => $"{a}\n{b}");

        return result;
    }

    public void PrintAndFlushErrors()
    {
        foreach (var error in _errors)
        {
            _writer.WriteLine(error);
        }
    }
}
