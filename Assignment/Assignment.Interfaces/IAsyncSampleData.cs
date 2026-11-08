using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Assignment;

public interface IAsyncSampleData
{
    // 1.
    IAsyncEnumerable<string> CsvRows { get; }

    // 2.
    IAsyncEnumerable<string> GetUniqueSortedListOfStatesGivenCsvRows();

    // 3. Returns Task<string> rather than string so callers can await it
    //    instead of blocking on the async file read.
    Task<string> GetAggregateSortedListOfStatesUsingCsvRows();

    // 4.
    IAsyncEnumerable<IPerson> People { get; }

    // 5.
    IAsyncEnumerable<(string FirstName, string LastName)> FilterByEmailAddress(Predicate<string> filter);

    // 6. Task<string> for the same reason as 3.
    Task<string> GetAggregateListOfStatesGivenPeopleCollection(IAsyncEnumerable<IPerson> people);
}