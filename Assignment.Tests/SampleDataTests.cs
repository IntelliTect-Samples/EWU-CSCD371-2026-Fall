using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

// Runs every test in SampleDataTestsBase against the synchronous SampleData.
[TestClass]
public class SampleDataTests : SampleDataTestsBase
{
    private readonly SampleData _sampleData = new();

    protected override Task<string[]> GetCsvRowsAsync()
        => Task.FromResult(_sampleData.CsvRows.ToArray());

    protected override Task<string[]> GetUniqueSortedListOfStatesGivenCsvRowsAsync()
        => Task.FromResult(_sampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    protected override Task<string> GetAggregateSortedListOfStatesUsingCsvRowsAsync()
        => Task.FromResult(_sampleData.GetAggregateSortedListOfStatesUsingCsvRows());

    protected override Task<IPerson[]> GetPeopleAsync()
        => Task.FromResult(_sampleData.People.ToArray());

    protected override Task<(string FirstName, string LastName)[]> FilterByEmailAddressAsync(Predicate<string> filter)
        => Task.FromResult(_sampleData.FilterByEmailAddress(filter).ToArray());

    protected override Task<string> GetAggregateListOfStatesGivenPeopleCollectionAsync(IEnumerable<IPerson> people)
        => Task.FromResult(_sampleData.GetAggregateListOfStatesGivenPeopleCollection(people));
}