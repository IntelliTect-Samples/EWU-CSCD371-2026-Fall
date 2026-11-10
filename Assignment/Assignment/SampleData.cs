using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1. Read all lines from the CSV file, skipping the header row.
    public virtual IEnumerable<string> CsvRows =>
        File.ReadAllLines("People.csv").Skip(1);

    // 2. Get a unique, alphabetically sorted list of states from the CSV rows.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() =>
        CsvRows
            .Select(row => row.Split(','))
            .Where(c => c.Length > 6 && !string.IsNullOrWhiteSpace(c[6]))
            .Select(c => c[6].Trim())
            .Distinct()
            .OrderBy(state => state, StringComparer.Ordinal);

    // 3. Get a comma-separated string of unique, alphabetically sorted states from the CSV rows.
    public string GetAggregateSortedListOfStatesUsingCsvRows() =>
        string.Join(", ", GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    // 4. Get a collection of IPerson objects parsed from the CSV rows, sorted by state, city, and zip code.
    public IEnumerable<IPerson> People =>
        CsvRows
            .Select(TryParsePerson)
            .OfType<Person>()
            .OrderBy(p => p.Address.State, StringComparer.Ordinal)
            .ThenBy(p => p.Address.City, StringComparer.Ordinal)
            .ThenBy(p => p.Address.Zip, StringComparer.Ordinal);
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

    // 5. Filter the collection of IPerson objects by email address using a provided predicate, returning a collection of tuples containing first and last names.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) =>
        People
            .Where(p => filter(p.EmailAddress))
            .Select(p => (p.FirstName, p.LastName));

    // 6.  Get a comma-separated string of unique states from a collection of IPerson objects.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) =>
        people
            .Select(p => p.Address.State)
            .Distinct()
            .Aggregate(string.Empty, (list, state) =>
                list.Length == 0 ? state : $"{list}, {state}");
    //Private helper method to parse a CSV row into a Person object. Returns null if the row is invalid.
    private static Person? TryParsePerson(string row)
    {
        string[] f = row.Split(',').Select(x => x.Trim()).ToArray();

        // Id, FirstName, LastName, Email, StreetAddress, City, State, Zip
        if (f.Length < 8 || f.Skip(1).Take(7).Any(string.IsNullOrEmpty))
            return null;

        return new Person(
            FirstName: f[1],
            LastName: f[2],
            Address: new Address(
                StreetAddress: f[4],
                City: f[5],
                State: f[6],
                Zip: f[7]),
            EmailAddress: f[3]);
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