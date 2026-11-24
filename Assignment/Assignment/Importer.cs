namespace Assignment;

public class Importer(HttpClientWrapper httpClient, TextWriter writer)
{
    private readonly List<string> _errors = []; 

    public Importer(HttpClientWrapper httpClient) : this(httpClient, Console.Out)
    {
    }

    public async Task<IReadOnlyList<Person>> ImportPeopleAsync(
        string user, CancellationToken cancellationToken = default)
    {
        DownloadResult download = await httpClient.DownloadPeopleCsvAsync(user, cancellationToken).ConfigureAwait(false);

        if (download.Status != DownloadStatus.Success)
        {
            throw new InvalidOperationException($"Download failed with status {download.Status} and error {download.Error}");
        }
        if (download.Csv is null)
        {
            throw new InvalidOperationException("Download succeeded but CSV is null");
        }

        List<Person> people = [];

        // Skip(1) skips the header row
        foreach (string line in download.Csv
            .Split('\n', StringSplitOptions.TrimEntries)
            .Skip(1))
        {
            ParseResult result = PersonParser.Parse(line);

            if (result.IsValid)
            {
                people.Add(result.Person!);
            }
            else
            {
                _errors.Add($"Error parsing line [{line}] - {string.Join(", ", result.Errors)}");
            }
        }

        return people;
    }

    public void PrintErrorsAndFlush()
    {
        writer.WriteLine($"{_errors.Count} invalid people removed:");
        _errors.ForEach(writer.WriteLine);
        writer.Flush();
    }
}