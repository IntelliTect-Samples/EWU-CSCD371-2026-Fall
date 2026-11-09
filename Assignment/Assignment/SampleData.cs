using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    public virtual IEnumerable<string> CsvRows =>
        File.ReadAllLines("People.csv").Skip(1);

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() =>
        CsvRows
            .Select(row => row.Split(','))
            .Where(c => c.Length > 6 && !string.IsNullOrWhiteSpace(c[6]))
            .Select(c => c[6].Trim())
            .Distinct()
            .OrderBy(state => state, StringComparer.Ordinal);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows() =>
        string.Join(", ", GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    // 4.
    public IEnumerable<IPerson> People =>
        CsvRows
            .Select(TryParsePerson)
            .OfType<Person>()
            .OrderBy(p => p.Address.State, StringComparer.Ordinal)
            .ThenBy(p => p.Address.City, StringComparer.Ordinal)
            .ThenBy(p => p.Address.Zip, StringComparer.Ordinal);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) =>
        People
            .Where(p => filter(p.EmailAddress))
            .Select(p => (p.FirstName, p.LastName));

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) =>
        people
            .Select(p => p.Address.State)
            .Distinct()
            .Aggregate(string.Empty, (list, state) =>
                list.Length == 0 ? state : $"{list}, {state}");

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
    }
}