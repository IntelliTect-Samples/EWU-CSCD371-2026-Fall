using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    // 1.
    public IEnumerable<string> CsvRows
    {
        get
        {
            return File.ReadLines("People.csv").Skip(1);
        }
    }

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
        string[] states = GetUniqueSortedListOfStatesGivenCsvRows().ToArray();
        return string.Join(",", states);
    }

    // 4.
    public IEnumerable<IPerson> People
    {
        get
        {
            return CsvRows.Select(row =>
            {
                var columns = row.Split(',');
                return new Person
                {
                    FirstName = columns[1],
                    LastName = columns[2],
                    EmailAddress = columns[3],
                    Address = new Address
                    {
                        StreetAddress = columns[4],
                        City = columns[5],
                        State = columns[6],
                        Zip = columns[7]
                    }
                };
            })
            .OrderBy(p => p.Address.State)
            .ThenBy(p => p.Address.City)
            .ThenBy(p => p.Address.Zip);
        }
    }

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        return People
            .Where(p => filter(p.EmailAddress))
            .Select(p => (p.FirstName, p.LastName));
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(IEnumerable<IPerson> people)
    {
        string[] states = people
            .Select(p => p.Address.State)
            .Distinct()
            .OrderBy(state => state)
            .ToArray();

        return states.Length == 0 ? string.Empty : states.Aggregate((current, next) => $"{current},{next}");
    }

internal sealed class Person : IPerson
{
    public string FirstName { get; set; } = "";
    public string LastName { get; set; } = "";
    public string EmailAddress { get; set; } = "";

    public IAddress Address { get; set; } =
        new Address();
}
internal sealed class Address : IAddress
{
    public string StreetAddress { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string Zip { get; set; } = "";


}
}