using System.Collections.Concurrent;

namespace Assignment;

public class Importer
{

    private readonly TextWriter _writer;

    public Importer(TextWriter writer = null!)
        => _writer = writer ?? Console.Out;

    private ConcurrentBag<string> _errorMessageCache = [];

    public async Task<IReadOnlyList<Person>> ImportPeopleAsync(CancellationToken cancellationToken = default)
    {
        string csv = await HttpClientWrapper.DownloadCsvAsync(cancellationToken);

        var rowLines = csv.Split("\n", StringSplitOptions.TrimEntries);

        List<Person> people = [];

        for (int i = 1; i < rowLines.Length; ++i)
        {
            string line = rowLines[i];
            ParseResult result = PersonParser.Parse(line);

            if (result.IsValid)
            {
                ArgumentNullException.ThrowIfNull(result.Person);
                people.Add(result.Person);
            }

            else
            {
                string errorMessage = string.Join(", ", result.Errors);
                _errorMessageCache.Add($"Error parsing line [{line}] - {errorMessage}\nResult removed");
            }

        }

        return people;
    }


    public void PrintErrorsAndFlush()
    {
        foreach (var error in _errorMessageCache)
        {
            _writer.WriteLine(error);
        }

        _errorMessageCache.Clear();
    }


}