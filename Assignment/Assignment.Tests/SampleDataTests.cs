using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests : SampleDataTestsBase
{
    private readonly SampleData _data = new();

    protected override Task<string[]> GetCsvRows()
        => Task.FromResult(_data.CsvRows.ToArray());

    protected override Task<string[]> GetUniqueSortedStates()
        => Task.FromResult(_data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray());

    protected override Task<string> GetAggregateSortedStates()
        => Task.FromResult(_data.GetAggregateSortedListOfStatesUsingCsvRows());

    protected override Task<IPerson[]> GetPeople()
        => Task.FromResult(_data.People.ToArray());

    protected override Task<(string FirstName, string LastName)[]> FilterByEmail(Predicate<string> filter)
        => Task.FromResult(_data.FilterByEmailAddress(filter).ToArray());

    protected override Task<string> GetAggregateStatesFor(IPerson[]? people)
        => Task.FromResult(_data.GetAggregateListOfStatesGivenPeopleCollection(people!));
}
