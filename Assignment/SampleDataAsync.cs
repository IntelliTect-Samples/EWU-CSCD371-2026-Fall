using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment;

public class SampleDataAsync : SampleDataServiceBase, IAsyncSampleData
{
    protected override IEnumerable<string> GetRawCsvRowsSync() => Enumerable.Empty<string>();

    private static async IAsyncEnumerable<string> GetRawCsvRowsAsync()
    {
        var lines = await Task.Run(() => File.ReadLines(SampleDirectory));
        foreach (var line in lines.Skip(1))
        {
            yield return line;
        }
    }

    // 1.
    public IAsyncEnumerable<string> CsvRows => GetRawCsvRowsAsync();

    // 2.
    public async IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        var states = CsvRows
            .Select(StringToPerson)
            .Select(p => p.Address.State)
            .Distinct();

        var sortedStates = await states.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToListAsync();

        foreach (var state in sortedStates)
        {
            yield return state;
        }
    }

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        var uniqueSortedStates = GetUniqueSortedListOfStatesGivenCsvRows().ToEnumerable();

        return string.Join(",", uniqueSortedStates);
    }

    // 4.
    public IAsyncEnumerable<IPerson> People => GetPeopleAsyncEnumerable();
    private async IAsyncEnumerable<IPerson> GetPeopleAsyncEnumerable()
    {
        var people = CsvRows.Select(StringToPerson);
        var peopleList = await people.ToListAsync();
        var sortedPeople = GetOrderedPeopleFromListSync(peopleList);

        foreach (var person in sortedPeople)
        {
            yield return person;
        }
    }

    // 5.
    public async IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        await foreach (var person in People)
        {
            if (filter(person.EmailAddress))
            {
                yield return (person.FirstName, person.LastName);
            }
        }
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people)
    {
        var peopleList = people.ToEnumerable();
        return AggregateStatesSync(peopleList);
    }
}