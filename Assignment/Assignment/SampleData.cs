using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    private static readonly string CsvFile =
    Path.Combine(
        AppContext.BaseDirectory,
        "People.csv");
    public IEnumerable<string> CsvRows =>
    File.ReadLines(CsvFile)
        .Skip(1);
    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        => CsvRows.Select(row => row.Split(',')[6]).Distinct().OrderBy(State => State);
    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => string.Join(",", GetUniqueSortedListOfStatesGivenCsvRows());
    // 4.
    public IEnumerable<IPerson> People => CsvRows.Select(row =>
    {
        string[] columns = row.Split(',');

        return new Person(columns[1],
            columns[2],
            columns[3],
            new Address(columns[4],
            columns[5],
            columns[6],
            columns[7]));
    }
    ).OrderBy(p => p.Address.State)
        .ThenBy(p => p.Address.City)
        .ThenBy(p => p.Address.Zip);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) => People.Where(person => filter(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName));

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) => People
        .Select(person => person.Address.State)
        .Distinct()
        .OrderBy(state => state)
        .Aggregate((a, b) => a + ", " + b);

}
