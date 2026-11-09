using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataAsyncTests : SampleDataTestsBase
{
    private readonly SampleDataAsync _data = new();

    protected override async Task<string[]> GetCsvRows()
        => await _data.CsvRows.ToArrayAsync();

    protected override async Task<string[]> GetUniqueSortedStates()
        => await _data.GetUniqueSortedListOfStatesGivenCsvRows().ToArrayAsync();

    protected override async Task<string> GetAggregateSortedStates()
        => await _data.GetAggregateSortedListOfStatesUsingCsvRows();

    protected override async Task<IPerson[]> GetPeople()
        => await _data.People.ToArrayAsync();

    protected override async Task<(string FirstName, string LastName)[]> FilterByEmail(Predicate<string> filter)
        => await _data.FilterByEmailAddress(filter).ToArrayAsync();

    protected override async Task<string> GetAggregateStatesFor(IPerson[]? people)
        => await _data.GetAggregateListOfStatesGivenPeopleCollection(people?.ToAsyncEnumerable()!);
}
