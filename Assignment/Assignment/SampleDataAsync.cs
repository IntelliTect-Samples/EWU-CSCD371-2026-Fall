using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment;

public class SampleDataAsync : IAsyncSampleData
{
    // 1.
    public IAsyncEnumerable<string> CsvRows =>
        File.ReadLinesAsync(SampleData.CsvPath)
            .Skip(1);

    // 2.
    public IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        => CsvRows
            .Select(SampleData.StateOf)
            .Distinct()
            .Order(StringComparer.Ordinal);

    // 3.
    public async Task<string> GetAggregateSortedListOfStatesUsingCsvRows()
        => SampleData.JoinStates(await GetUniqueSortedListOfStatesGivenCsvRows().ToArrayAsync());

    // 4.
    public IAsyncEnumerable<IPerson> People =>
        CsvRows
            .Select(row => (IPerson)SampleData.ParsePerson(row))
            .OrderBy(SampleData.ByState, StringComparer.Ordinal)
            .ThenBy(SampleData.ByCity, StringComparer.Ordinal)
            .ThenBy(SampleData.ByZip, StringComparer.Ordinal);

    // 5.
    public IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return People
            .Where(person => filter(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName));
    }

    // 6.
    public Task<string> GetAggregateListOfStatesGivenPeopleCollection(
        IAsyncEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        return people
            .Select(SampleData.ByState)
            .Distinct()
            .Order(StringComparer.Ordinal)
            .AggregateAsync(string.Empty, SampleData.AppendState)
            .AsTask();
    }
}
