using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows => File.ReadLines(Path.Combine(AppContext.BaseDirectory, "People.csv")).Skip(1);

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() 
        => CsvRows
            .Select(row => row.Split(',')[6])
            .Distinct(StringComparer.Ordinal)
            .OrderBy(state => state, StringComparer.Ordinal);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => string.Join(",", GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    // 4.
    public IEnumerable<IPerson> People => CsvRows
        .Select(ParsePerson)
        .OrderBy(person => person.Address.State, StringComparer.Ordinal)
        .ThenBy(person => person.Address.City, StringComparer.Ordinal)
        .ThenBy(person => person.Address.Zip, StringComparer.Ordinal);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) => throw new NotImplementedException();

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) => throw new NotImplementedException();

    private static IPerson ParsePerson(string row)
    {
        string[] columns = row.Split(',');
        if (columns.Length != 8)
        {
            throw new FormatException("A People.csv row must contain exactly 8 columns.");
        }

        return new Person(
            columns[1],
            columns[2],
            columns[3],
            new Address(columns[4], columns[5], columns[6], columns[7]));
    }
}
