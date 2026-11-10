using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows => File.ReadLines("People.csv").Skip(1);

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() =>
        CsvRows.Select(row => ParseRow(row)[6])
               .Distinct(StringComparer.Ordinal)
               .OrderBy(state => state, StringComparer.Ordinal);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => String.Join(",", GetUniqueSortedListOfStatesGivenCsvRows());

    // 4.
    public IEnumerable<IPerson> People =>  
        CsvRows.Select(row =>
        {
            string[] columns = ParseRow(row);
            return (IPerson)new Person(
                columns[1], columns[2], columns[3],
                new Address(columns[4], columns[5], columns[6], columns[7]));
        })
        .OrderBy(person => person.Address.State, StringComparer.Ordinal)
        .ThenBy(person => person.Address.City, StringComparer.Ordinal)
        .ThenBy(person => person.Address.Zip, StringComparer.Ordinal);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);
        return People.Where(person => filter(person.EmailAddress))
                     .Select(person => (person.FirstName, person.LastName));
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(IEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);
        return people.Select(person => person.Address.State)
                     .Distinct(StringComparer.Ordinal)
                     .OrderBy(state => state, StringComparer.Ordinal)
                     .Aggregate(string.Empty, (result, state) =>
                         result.Length == 0 ? state : result + "," + state);
    }

    private static string[] ParseRow(string row)
    {
        string[] columns = row.Split(',');
        if (columns.Length != 8)
        {
            throw new FormatException("Each People.csv row must contain exactly eight columns.");
        }
        return columns;
    }
}