namespace Assignment;

public interface IAsyncSampleData
{
    IAsyncEnumerable<string> CsvRows { get; }
    IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows();
    Task<string> GetAggregateSortedListOfStatesUsingCsvRows();
    IAsyncEnumerable<IPerson> People { get; }
    IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter);
    Task<string> GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people);
}
