using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // Parsing and formatting shared with SampleDataAsync.
    internal static string CsvPath => Path.Combine(AppContext.BaseDirectory, "People.csv");

    internal static string StateOf(string row) => row.Split(',')[6];

    internal static Person ParsePerson(string row)
    {
        string[] columns = row.Split(',');

        return new Person(
            columns[1],
            columns[2],
            new Address(columns[4], columns[5], columns[6], columns[7]),
            columns[3]);
    }

    internal static string ByState(IPerson person) => person.Address.State;
    internal static string ByCity(IPerson person) => person.Address.City;
    internal static string ByZip(IPerson person) => person.Address.Zip;

    internal static IOrderedEnumerable<IPerson> SortPeople(IEnumerable<IPerson> people)
        => people
            .OrderBy(ByState, StringComparer.Ordinal)
            .ThenBy(ByCity, StringComparer.Ordinal)
            .ThenBy(ByZip, StringComparer.Ordinal);

    internal static string JoinStates(IEnumerable<string> states) => string.Join(", ", states);

    internal static string AppendState(string list, string state)
        => list.Length == 0 ? state : $"{list}, {state}";

    // 1.
    public IEnumerable<string> CsvRows =>
        File.ReadLines(CsvPath)
            .Skip(1);

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
        => CsvRows
            .Select(StateOf)
            .Distinct()
            .Order(StringComparer.Ordinal);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => JoinStates(GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    // 4.
    public IEnumerable<IPerson> People =>
        SortPeople(CsvRows.Select(ParsePerson));

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
            .Select(ByState)
            .Distinct()
            .Order(StringComparer.Ordinal)
            .Aggregate(string.Empty, AppendState);
    }
}
