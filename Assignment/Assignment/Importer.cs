namespace Assignment;

public class Importer(HttpClientWrapper httpClient, TextWriter writer)
{
    private readonly List<string> _errors = [];

    public Importer(HttpClientWrapper httpClient) : this(httpClient, Console.Out)
    {
    }

    public async Task<PersonParseResultReport> ImportPeopleAsync(
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

        var parsed = PersonParser.ParseCsv(download.Csv);
        return parsed;
    }

    public void PrintErrorsAndFlush()
    {
        writer.WriteLine($"{_errors.Count} invalid people removed:");
        _errors.ForEach(writer.WriteLine);
        writer.Flush();
    }
}