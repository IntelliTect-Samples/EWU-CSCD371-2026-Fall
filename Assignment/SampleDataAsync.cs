using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment;

public class SampleDataAsync : IAsyncSampleData
{
    // 1. File.ReadLinesAsync reads without blocking a thread and closes the
    //    file when enumeration finishes or is abandoned.
    public IAsyncEnumerable<string> CsvRows => File.ReadLinesAsync(PeopleCsv.FilePath).Skip(1);

    // 2.
    public IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        => CsvRows
            .Select(PeopleCsv.GetState)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(state => state, StringComparer.Ordinal);

    // 3.
    public async Task<string> GetAggregateSortedListOfStatesUsingCsvRows()
        => string.Join(",", await GetUniqueSortedListOfStatesGivenCsvRows().ToArrayAsync().ConfigureAwait(false));

    // 4.
    public IAsyncEnumerable<IPerson> People => CsvRows
        .Select(PeopleCsv.ParsePerson)
        .OrderBy(person => person.Address.State, StringComparer.Ordinal)
        .ThenBy(person => person.Address.City, StringComparer.Ordinal)
        .ThenBy(person => person.Address.Zip, StringComparer.Ordinal);

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
    public async Task<string> GetAggregateListOfStatesGivenPeopleCollection(
        IAsyncEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        return await people
            .Select(person => person.Address.State)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(state => state, StringComparer.Ordinal)
            .AggregateAsync(string.Empty, PeopleCsv.AppendState)
            .ConfigureAwait(false);
    }
}