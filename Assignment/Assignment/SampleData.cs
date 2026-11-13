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
    {
        return CsvRows
            .Select(row => row.Split(',')[6])
            .Distinct()
            .OrderBy(state => state);
    }

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        var states = GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        return string.Join(", ", states);
    }

    // 4.
    public IEnumerable<IPerson> People =>
    CsvRows
        .Select(row => row.Split(','))
        .Select(columns => (IPerson)new Person(
            columns[1],
            columns[2],
            columns[3],
            new Address(
                columns[4],
                columns[5],
                columns[6],
                columns[7])))
        .OrderBy(person => person.Address.State)
        .ThenBy(person => person.Address.City)
        .ThenBy(person => person.Address.Zip);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
     Predicate<string> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return People
            .Where(person => filter(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName));
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
     IEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        return people
            .Select(person => person.Address.State)
            .Distinct()
            .OrderBy(state => state)
            .Aggregate(string.Empty, (result, state) =>
                result.Length == 0 ? state : $"{result}, {state}");
    }
}
