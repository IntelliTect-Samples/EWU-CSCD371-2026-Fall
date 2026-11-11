using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public interface ISampleData
{
    // 1.
    IEnumerable<string> CsvRows
    {
        get
        {
            using StreamReader reader = new("People.csv");
            reader.ReadLine();
            List<string> lines = [];
            while (reader.ReadLine() is string line)
            {
                lines.Add(line);
            }
            return lines.Skip(1); // Skip the header row
        }
    }

    // 2.
    IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        return CsvRows
            .Select(row => row.Split(',')[6]) // Assuming the state is in the 7th column (index 6)
            .Distinct()
            .OrderBy(state => state);
    }

    // 3.
    string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        return string.Join(", ", GetUniqueSortedListOfStatesGivenCsvRows().Select(state => state.ToArray()));
    }
    private static string? Column(string[] columns, int index) =>
        index < columns.Length && !string.IsNullOrWhiteSpace(columns[index]) ? columns[index].Trim() : null;

    // 5.
    IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(IEnumerable<IPerson> people, Predicate<string> filter) => people
        .Where(p => filter(p.EmailAddress))
        .Select(p => (p.FirstName, p.LastName));
    // 6.
    string GetAggregateListOfStatesGivenPeopleCollection(IEnumerable<IPerson> people) => people
        .Select(p => p.Address.State)
        .Distinct()
        .Aggregate(string.Empty, (list, state) => list.Length == 0 ? state : $"{list}, {state}");
}
