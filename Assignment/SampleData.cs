using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    public virtual IEnumerable<string> CsvRows =>
        File.ReadAllLines("People.csv").Skip(1);

    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() =>
        CsvRows
            .Select(ParseRow)
            .Where(columns => columns.Length > 6 && !string.IsNullOrWhiteSpace(columns[6]))
            .Select(columns => columns[6].Trim())
            .Distinct(StringComparer.Ordinal)
            .OrderBy(state => state, StringComparer.Ordinal);

    public string GetAggregateSortedListOfStatesUsingCsvRows() =>
        string.Join(", ", GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    public IEnumerable<IPerson> People =>
        CsvRows
            .Select(TryParsePerson)
            .OfType<Person>()
            .Cast<IPerson>()
            .OrderBy(person => person.Address.State, StringComparer.Ordinal)
            .ThenBy(person => person.Address.City, StringComparer.Ordinal)
            .ThenBy(person => person.Address.Zip, StringComparer.Ordinal);

    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        return People
            .Where(person => filter(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName));
    }

    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people)
    {
        ArgumentNullException.ThrowIfNull(people);

        return people
            .Select(person => person.Address.State)
            .Distinct(StringComparer.Ordinal)
            .OrderBy(state => state, StringComparer.Ordinal)
            .Aggregate(string.Empty, (result, state) =>
                result.Length == 0 ? state : $"{result}, {state}");
    }

    private static string[] ParseRow(string row)
    {
        ArgumentNullException.ThrowIfNull(row);

        string[] columns = row.Split(',');
        if (columns.Length != 8)
        {
            throw new FormatException("Each People.csv row must contain exactly eight columns.");
        }

        return columns;
    }

    private static Person? TryParsePerson(string row)
    {
        try
        {
            string[] columns = ParseRow(row);

            if (columns.Skip(1).Take(7).Any(string.IsNullOrWhiteSpace))
            {
                return null;
            }

            return new Person(
                FirstName: columns[1],
                LastName: columns[2],
                EmailAddress: columns[3],
                Address: new Address(
                    StreetAddress: columns[4],
                    City: columns[5],
                    State: columns[6],
                    Zip: columns[7]));
        }
        catch (FormatException)
        {
            return null;
        }
    }
}