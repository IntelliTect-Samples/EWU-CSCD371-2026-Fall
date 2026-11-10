using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : SampleDataBase, ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows =>
    File.ReadLines(CsvFile)
        .Skip(1);
    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        => CsvRows.Select(row => GetState(row)).Distinct().OrderBy(State => State);
    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => string.Join(", ", GetUniqueSortedListOfStatesGivenCsvRows());
    // 4.
    public IEnumerable<IPerson> People => CsvRows.Select(row => ParsePerson(row)
    ).OrderBy(p => p.Address.State)
        .ThenBy(p => p.Address.City)
        .ThenBy(p => p.Address.Zip);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) => People.Where(person => filter(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName));

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) => people
        .Select(person => person.Address.State)
        .Distinct()
        .OrderBy(state => state)
        .Aggregate((a, b) => $"{a}, {b}");

}
