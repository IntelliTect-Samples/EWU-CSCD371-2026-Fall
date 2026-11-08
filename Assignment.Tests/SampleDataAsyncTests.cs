using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

// Runs every test in SampleDataTestsBase against SampleDataAsync.
[TestClass]
public class SampleDataAsyncTests : SampleDataTestsBase
{
    private readonly SampleDataAsync _sampleData = new();

    protected override Task<string[]> GetCsvRowsAsync()
        => _sampleData.CsvRows.ToArrayAsync().AsTask();

    protected override Task<string[]> GetUniqueSortedListOfStatesGivenCsvRowsAsync()
        => _sampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToArrayAsync().AsTask();

    protected override Task<string> GetAggregateSortedListOfStatesUsingCsvRowsAsync()
        => _sampleData.GetAggregateSortedListOfStatesUsingCsvRows();

    protected override Task<IPerson[]> GetPeopleAsync()
        => _sampleData.People.ToArrayAsync().AsTask();

    protected override Task<(string FirstName, string LastName)[]> FilterByEmailAddressAsync(Predicate<string> filter)
        => _sampleData.FilterByEmailAddress(filter).ToArrayAsync().AsTask();

    // Passes null straight through so the null test exercises SampleDataAsync's
    // own argument check rather than ToAsyncEnumerable's.
    protected override Task<string> GetAggregateListOfStatesGivenPeopleCollectionAsync(IEnumerable<IPerson> people)
        => _sampleData.GetAggregateListOfStatesGivenPeopleCollection(people?.ToAsyncEnumerable()!);
}