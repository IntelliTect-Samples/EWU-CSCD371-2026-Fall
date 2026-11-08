using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows => File.ReadLines(PeopleCsv.FilePath).Skip(1);

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        => CsvRows
            .Select(PeopleCsv.GetState)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(state => state, StringComparer.Ordinal);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => string.Join(",", GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    // 4.
    public IEnumerable<IPerson> People => CsvRows
        .Select(PeopleCsv.ParsePerson)
        .OrderBy(person => person.Address.State, StringComparer.Ordinal)
        .ThenBy(person => person.Address.City, StringComparer.Ordinal)
        .ThenBy(person => person.Address.Zip, StringComparer.Ordinal);

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
            .Distinct(StringComparer.Ordinal)
            .OrderBy(state => state, StringComparer.Ordinal)
            .Aggregate(string.Empty, PeopleCsv.AppendState);
    }
}