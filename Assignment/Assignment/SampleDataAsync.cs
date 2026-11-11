using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment;

public class SampleDataAsync : SampleDataBase, IAsyncSampleData
{
    private static async IAsyncEnumerable<string> ReadCsvRowsAsync(string filePath)
    {
        using StreamReader sr = new(filePath);
        await sr.ReadLineAsync();
        string? s;

        while ((s = await sr.ReadLineAsync()) != null)
        {
            yield return s;
        }
    }

    private async IAsyncEnumerable<IPerson> GetPeopleAsync()
    {
        await foreach (var row in CsvRows)
        {
            yield return ParsePerson(row);
        }
    }

    private static async Task<string> JoinAsync(string seperator, IAsyncEnumerable<string> strings)
    {
        string result = "";
        bool isFirst = true;

        await foreach (var s in strings)
        {
            result = isFirst ? s : string.Join(seperator, [result, s]);
            isFirst = false;
        }

        return result;
    }


    public IAsyncEnumerable<string> CsvRows => ReadCsvRowsAsync(CsvFile);

    public IAsyncEnumerable<IPerson> People => GetPeopleAsync().OrderBy(p => p.Address.State)
        .ThenBy(p => p.Address.City)
        .ThenBy(p => p.Address.Zip);

    public IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
        => People.Where(async (person, ct) => await Task.FromResult(filter(person.EmailAddress)))
                .Select(person => (person.FirstName, person.LastName));

    public async Task<string> GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people)
    {
        var states = people.Select(person => person.Address.State).Distinct().OrderBy(state => state);
        return await JoinAsync(", ", states);
    }

    public async Task<string> GetAggregateSortedListOfStatesUsingCsvRows()
    {
        var states = GetUniqueSortedListOfStatesGivenCsvRows();
        return await JoinAsync(", ", states);
    }

    public IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        return CsvRows.Select(row => GetState(row)).Distinct().OrderBy(State => State);
    }

}
