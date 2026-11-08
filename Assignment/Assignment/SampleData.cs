using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows =>
        File.ReadLines(Path.Combine(AppContext.BaseDirectory, "People.csv"))
            .Skip(1);

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        => CsvRows
            .Select(row => row.Split(',')[6])
            .Distinct()
            .Order(StringComparer.Ordinal);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => string.Join(", ", GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    // 4.
    public IEnumerable<IPerson> People =>
        CsvRows
            .Select(row => row.Split(','))
            .Select(columns => new Person(
                columns[1],
                columns[2],
                new Address(columns[4], columns[5], columns[6], columns[7]),
                columns[3]))
            .OrderBy(person => person.Address.State, StringComparer.Ordinal)
            .ThenBy(person => person.Address.City, StringComparer.Ordinal)
            .ThenBy(person => person.Address.Zip, StringComparer.Ordinal);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) => throw new NotImplementedException();

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) => throw new NotImplementedException();
}
