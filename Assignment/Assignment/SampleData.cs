using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1. 
    public IEnumerable<string> CsvRows
    {
        get
        {
            string baseDir = AppContext.BaseDirectory;
            string csvPath = Path.Combine(baseDir, "People.csv");
            if (!File.Exists(csvPath))
            {
                throw new FileNotFoundException($"People.csv not found at '{csvPath}'.");
            }
            return File.ReadLines(csvPath).Skip(1).ToArray();
        }
    }



    // 2. 
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        => GetUniqueSortedListOfStates(CsvRows).ToArray();



    // 3. 
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        string[] states = GetUniqueSortedListOfStatesGivenCsvRows().Select(state => state).ToArray();
        return string.Join(", ", states);
    }



    // 4. 
    public IEnumerable<IPerson> People => CsvRows
        .Select(ParsePerson)
        .OrderBy(p => p.Address.State, StringComparer.OrdinalIgnoreCase)
        .ThenBy(p => p.Address.City, StringComparer.OrdinalIgnoreCase)
        .ThenBy(p => p.Address.Zip, StringComparer.OrdinalIgnoreCase).ToArray();



    // 5. 
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
        =>  People.Where(p => filter(p.EmailAddress)).Select(p => (p.FirstName, p.LastName)).ToArray();



    // 6. 
    public string GetAggregateListOfStatesGivenPeopleCollection(IEnumerable<IPerson> people)
    {
        IEnumerable<string> states = people.Select(p => p.Address.State)
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s, StringComparer.OrdinalIgnoreCase);
        return states.Aggregate(string.Empty, (acc, s) => string.IsNullOrEmpty(acc) ? s : $"{acc}, {s}");
    }

    // --- Helper methods ---
    private static IPerson ParsePerson(string csv)
    {
        string[] parts = csv.Split(',', StringSplitOptions.None);
        if (parts.Length < 8)
            throw new FormatException($"Invalid CSV row: {csv}");
        string first = parts[1].Trim();
        string last = parts[2].Trim();
        string email = parts[3].Trim();
        string street = parts[4].Trim();
        string city = parts[5].Trim();
        string state = parts[6].Trim();
        string zip = parts[7].Trim();
        return new Person(first, last, new Address(street, city, state, zip), email);
    }

    public static IEnumerable<string> GetUniqueSortedListOfStates(IEnumerable<string> rows)
        => rows.Select(r => r.Split(',', StringSplitOptions.None))
               .Where(parts => parts.Length >= 7)
               .Select(parts => parts[6].Trim())
               .Where(s => !string.IsNullOrWhiteSpace(s))
               .Distinct(StringComparer.OrdinalIgnoreCase)
               .OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToArray();
}
