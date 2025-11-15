using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Assignment;

public class SampleData : ISampleData
{
    private const string SampleDirectory = "People.csv";

    // 1.
    public IEnumerable<string> CsvRows { 
        get {
            return File.ReadLines(SampleDirectory).Skip(1);
        } 
    }

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        IEnumerable<string> csvTemp = CsvRows.Distinct();
        return csvTemp.OrderBy(n => n);
    }

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        IEnumerable<string> csvTemp = GetUniqueSortedListOfStatesGivenCsvRows();

        return string.Join(",", csvTemp);
    }

    // 4.
    public IEnumerable<IPerson> People
    {
        get 
        {
            return CsvRows.Select(StringToPerson)
                .OrderBy(person => person.Address.State)
                .ThenBy(person => person.Address.City)
                .ThenBy(person => person.Address.Zip);
        }
    }

    public IPerson StringToPerson(string input)
    {
        if(input == null) throw new ArgumentNullException(nameof(input));

        string[] split = input.Split(',');

        if(split.Length <= 7) throw new InvalidDataException($"{nameof(input)} : {input}");

        Address address = new Address(split[4], split[5], split[6], split[7]);
        return new Person(split[1], split[2], address, split[3]);
    }

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        return People.Where(person => filter(person.EmailAddress)).Select(p => (p.FirstName,p.LastName));
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) => throw new NotImplementedException();
}
