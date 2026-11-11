using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Tests;


[TestClass]
public class SampleDataTestsAsync : SampleDataTestsBase
{
    private readonly SampleDataAsync _sampleData = new();

    protected override async Task<IReadOnlyList<string>> CsvRows()
        => await _sampleData.CsvRows.ToListAsync();

    protected override async Task<IReadOnlyList<(string FirstName, string LastName)>> FilterByEmailAddress(Predicate<string> filter)
        => await _sampleData.FilterByEmailAddress(filter).ToListAsync();

    protected override async Task<string> GetAggregateSortedListOfStatesUsingCsvRows()
        => await _sampleData.GetAggregateSortedListOfStatesUsingCsvRows();

    protected override async Task<IReadOnlyList<string>> GetUniqueSortedListOfStatesGivenCsvRows()
        => await _sampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToListAsync();

    protected override async Task<IReadOnlyList<IPerson>> People()
        => await _sampleData.People.ToListAsync();

    [TestMethod]
    public async Task TestAggregateListOfStatesGivenPeopleCollection()
    {
        var people = _sampleData.People;
        string expected = await _sampleData.GetAggregateSortedListOfStatesUsingCsvRows();

        string actual = await _sampleData.GetAggregateListOfStatesGivenPeopleCollection(people);

        Assert.AreEqual<string>(
            expected,
            actual);
    }
}