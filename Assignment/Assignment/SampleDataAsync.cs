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
        StreamReader sr = new(filePath);
        await sr.ReadLineAsync();
        string? s;

        while ((s = await sr.ReadLineAsync()) != null)
        {
            yield return s;
        }

        sr.Close();
        sr.DiscardBufferedData();
        sr.Dispose();
    }

    private async IAsyncEnumerable<IPerson> GetPeopleAsync()
    {
        await foreach (var row in CsvRows)
        {
            yield return ParsePerson(row);
        }
    }

    public IAsyncEnumerable<string> CsvRows => ReadCsvRowsAsync(CsvFile);

    public IAsyncEnumerable<IPerson> People => GetPeopleAsync().OrderBy(p => p.Address.State)
        .ThenBy(p => p.Address.City)
        .ThenBy(p => p.Address.Zip);//dup code

    public IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
        => People.Where(async (person, ct) => await Task.FromResult(filter(person.EmailAddress)))
                .Select(person => (person.FirstName, person.LastName));

    public string GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people)
    {
        var states = Task.Run(() => people.Select(person => person.Address.State).Distinct().OrderBy(state => state).ToListAsync().AsTask()).GetAwaiter().GetResult();
        return string.Join(", ", states);
    }

    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        var states = GetUniqueSortedListOfStatesGivenCsvRows();
        List<string> list = Task.Run(() => states.ToListAsync().AsTask()).GetAwaiter().GetResult();
        return string.Join(", ", list);
    }

    public IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        return CsvRows.Select(row => GetState(row)).Distinct().OrderBy(State => State); // duplicate code
    }

}
