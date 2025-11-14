using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Assignment;

public class SampleData : ISampleData
{
    private const string SampleDirectory = "People.csv";

    // 1.
    public IEnumerable<string> CsvRows { 
        get {
            List<string> list = new List<string>();
            using (StreamReader reader = new StreamReader(SampleDirectory))
            {
                string? line = reader.ReadLine();
                while(line != null)
                {
                    list.Add(line);
                    line = reader.ReadLine();
                } 
            }

            return list.Skip(1);
        } }

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows() 
        => throw new NotImplementedException();

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
        => throw new NotImplementedException();

    // 4.
    public IEnumerable<IPerson> People => throw new NotImplementedException();

    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(
        Predicate<string> filter) => throw new NotImplementedException();

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(
        IEnumerable<IPerson> people) => throw new NotImplementedException();
}
