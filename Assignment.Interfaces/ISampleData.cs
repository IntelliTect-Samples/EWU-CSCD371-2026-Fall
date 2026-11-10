using System;
using System.Collections.Generic;

namespace Assignment;

public interface ISampleData
{
    IEnumerable<string> CsvRows { get; }

    IEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows();

    string GetAggregateSortedListOfStatesUsingCsvRows();

    IEnumerable<IPerson> People { get; }

    IEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter);

    string GetAggregateListOfStatesGivenPeopleCollection(IEnumerable<IPerson> people);

}
