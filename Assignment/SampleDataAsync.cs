using System;
using System.Collections.Generic;
using System.IO;
using System.Linq; 
using System.Threading.Tasks;

namespace Assignment;

public class SampleDataAsync : IAsyncSampleData
{
    private readonly string _csvFilePath;

    public SampleDataAsync(string csvFilePath)
    {
        _csvFilePath = csvFilePath;
    }

    public IAsyncEnumerable<string> CsvRows => ReadLinesAsync(_csvFilePath);

    // FIX: CA1822 - Marked method as static
    private static async IAsyncEnumerable<string> ReadLinesAsync(string path) 
    {
        using var reader = new StreamReader(path);
        await reader.ReadLineAsync(); // Skip header
        
        while (!reader.EndOfStream)
        {
            var line = await reader.ReadLineAsync();
            if (line != null) yield return line;
        }
    }

    public async IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        var states = new HashSet<string>();
        
        await foreach (var row in CsvRows)
        {
            // Calls shared CsvParser
            states.Add(CsvParser.ParseStateFromCsvRow(row)); 
        }

        foreach (var state in states.OrderBy(s => s))
        {
            yield return state;
        }
    }

    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        var states = GetUniqueSortedListOfStatesGivenCsvRows().ToEnumerable().ToList();
        return string.Join(", ", states);
    }

    public IAsyncEnumerable<IPerson> People
    {
        get
        {
            return CsvRows
                .Select(CsvParser.ParsePersonFromCsvRow)
                .OrderBy(p => p.Address.State)
                .ThenBy(p => p.Address.City)
                .ThenBy(p => p.Address.Zip);
        }
    }

    public IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        return People
            .Where(p => filter(p.EmailAddress))
            .Select(p => (p.FirstName, p.LastName));
    }

    public string GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people)
    {
        var distinctStates = people.ToEnumerable()
            .Select(p => p.Address.State)
            .Distinct()
            .OrderBy(s => s)
            .ToList();

        // FIX: CA1860 - Prefer comparing 'Count' to 0
        if (distinctStates.Count == 0) return string.Empty;
        return distinctStates.Aggregate((current, next) => $"{current}, {next}");
    }
}