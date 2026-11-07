using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public interface ISampleData
{
    // 1.
    IEnumerable<string> CsvRows {
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
    IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows();

    // 3.
    string GetAggregateSortedListOfStatesUsingCsvRows();

    // 4.
    IEnumerable<IPerson> People { get; }

    // 5.
    IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter);

    // 6.
    string GetAggregateListOfStatesGivenPeopleCollection(IEnumerable<IPerson> people);

}
