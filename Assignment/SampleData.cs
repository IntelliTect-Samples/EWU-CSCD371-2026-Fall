using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData(string filePath = "People.csv") : ISampleData
{
    public IEnumerable<string> CsvRows =>
        File.ReadLines(filePath)
            .Skip(1);

    // CSV Columns are defined as:
    // Id, FirstName, LastName, Email, StreetAddress, City, State, Zip
    
    // State is column 7 (index 6)
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() 
        => CsvRows
            .Select(row => row.Split(',')[6])
            .Distinct()
            .OrderBy(state => state);

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => string.Join(",", GetUniqueSortedListOfStatesGivenCsvRows());

    // 4.
    public IEnumerable<IPerson> People => 
        CsvRows
            .Select(row => row.Split(','))
            .Select(IPerson (columns) =>
                new Person(
                    firstName: columns[1],
                    lastName: columns[2],
                    emailAddress: columns[3],
                    
                    address: new Address(
                        streetAddress: columns[4],
                        city: columns[5],
                        state:columns[6],
                        zip:columns[7]
                    )
                )
            ).OrderBy(person => person.Address.State)
            .ThenBy(person => person.Address.City)
            .ThenBy(person => person.Address.Zip);

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) => 
            People
                .Where(person => filter(person.EmailAddress))
                .Select(person => (person.FirstName, person.LastName));

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) => 
            people
                .Select(person => person.Address.State)
                .Distinct()
                .OrderBy(state => state)
                .DefaultIfEmpty(string.Empty)
                .Aggregate((current, next) => 
                    string.IsNullOrEmpty(current)
                    ? next
                    : $"{current},{next}");
}
