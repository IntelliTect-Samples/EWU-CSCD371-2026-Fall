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
        DownloadResult download = await httpClient.DownloadPeopleCsvAsync(user, cancellationToken);

        if (download.Status != DownloadStatus.Success)
        {
            _errors.Add($"Download failed: {download.Status} {download.Error?.Message}");
            return [];
        }
        if (download.Csv is null)
        {
            _errors.Add("Download returned null CSV");
            return [];
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
        _errors.ForEach(writer.WriteLine);
        _errors.Clear();
    }
}