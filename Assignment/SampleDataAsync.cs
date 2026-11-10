namespace Assignment;

public class SampleDataAsync : IAsyncSampleData
{
    private readonly IAsyncEnumerable<string>? _CsvRows;

    public IAsyncEnumerable<string> CsvRows
    {
        get => _CsvRows ?? File.ReadLinesAsync(Path.Combine(AppContext.BaseDirectory, "People.csv"))
            .Skip(1).Where(row => !string.IsNullOrWhiteSpace(row));
        init => _CsvRows = value ?? throw new ArgumentNullException(nameof(value));
    }

    public IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() =>
        EnumerateAsync(data => data.GetUniqueSortedListOfStatesGivenCsvRows());

    public Task<string> GetAggregateSortedListOfStatesUsingCsvRows() =>
        QueryAsync(data => data.GetAggregateSortedListOfStatesUsingCsvRows());

    public IAsyncEnumerable<IPerson> People => EnumerateAsync(data => data.People);

    public IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return EnumerateAsync(data => data.FilterByEmailAddress(filter));
    }

    public async Task<string> GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);
        IPerson[] values = await people.ToArrayAsync();

        return await Task.Run(() => new SampleData { CsvRows = [] }.GetAggregateListOfStatesGivenPeopleCollection(values));
    }

    private async Task<T> QueryAsync<T>(Func<SampleData, T> query)
    {
        string[] rows = await CsvRows.ToArrayAsync();

        return await Task.Run(() => query(new SampleData { CsvRows = rows }));
    }

    private async IAsyncEnumerable<T> EnumerateAsync<T>(Func<SampleData, IEnumerable<T>> query)
    {
        foreach (T item in await QueryAsync(data => query(data).ToArray()))
        {
            yield return item;
        }
    }
}
