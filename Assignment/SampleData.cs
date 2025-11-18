using Microsoft.VisualBasic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;

namespace Assignment;

public class SampleData : SampleDataServiceBase, ISampleData
{
    protected override IEnumerable<string> GetRawCsvRowsSync()
    {
        return File.ReadLines(SampleDirectory);
    }

    // 1.
    public IEnumerable<string> CsvRows {
        get {
            return GetRawCsvRowsSync().Skip(1);
        }
    }

    // 2.
    public IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        return CsvRows.
            Select(StringToPerson)
            .Select(p => $"{p.Address.State}")
            .Distinct()
            .OrderBy(n => n, StringComparer.OrdinalIgnoreCase);
    }

    // 3.
    public string GetAggregateSortedListOfStatesUsingCsvRows()
    {
        return string.Join(",", GetUniqueSortedListOfStatesGivenCsvRows());
    }

    // 4.
    public IEnumerable<IPerson> People
    {
        get
        {
            var people = CsvRows.Select(StringToPerson);
            return GetOrderedPeopleFromListSync(people);
        }
    }
    
    // 5.
    public IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter)
    {
        return People.Where(person => filter(person.EmailAddress)).Select(p => (p.FirstName, p.LastName));
    }

    // 6.
    public string GetAggregateListOfStatesGivenPeopleCollection(IEnumerable<IPerson> people)
    {
        return AggregateStatesSync(people);
    }
        
}
