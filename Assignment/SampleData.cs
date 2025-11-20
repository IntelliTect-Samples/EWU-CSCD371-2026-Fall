using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    public virtual IEnumerable<string> CsvRows =>
        File.ReadLines("People.csv").Skip(1);

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() =>
        CsvRows
            .Select(row => row.Split(',')[6])
            .Distinct()
            .OrderBy(state => state);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => string.Join(",", GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    // 4.
    public IEnumerable<IPerson> People => CsvRows.Select(row =>
    {
        string[] columns = row.Split(',');
        Address address = new(
            streetAddress: columns[4],
            city: columns[5],
            state: columns[6],
            zip: columns[7]);
        return new Person(
            firstName: columns[1],
            lastName: columns[2],
            address: address,
            emailAddress: columns[3]);
    }).OrderBy(person => person.Address.State).ThenBy(person => person.Address.City).ThenBy(person => person.Address.Zip);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) => People.Where(p => filter(p.EmailAddress)).Select(p => (p.FirstName, p.LastName));

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) => people
            .Select(people => people.Address.State)
            .Distinct()
            .OrderBy(state => state)
            .Aggregate((states, state) => states + "," + state);
}
