using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Assignment.Tests;

[TestClass]
public class SampleDataTests : SampleDataTestsBase
{
    private readonly ISampleData _sampleData =
        new SampleData();

    [TestMethod]
    public void TestAggregateListOfStatesGivenPeopleCollection()
    {
        string expected =
            _sampleData
                .GetAggregateSortedListOfStatesUsingCsvRows();

        string actual =
            _sampleData
                .GetAggregateListOfStatesGivenPeopleCollection(
                    _sampleData.People);

        Assert.AreEqual<string>(
            expected,
            actual);
    }

    protected override Task<IReadOnlyList<string>> CsvRows()
    {
        IReadOnlyList<string> csvRows = _sampleData.CsvRows.ToList();
        return Task.FromResult(csvRows);
    }

    protected override Task<IReadOnlyList<(string FirstName, string LastName)>> FilterByEmailAddress(Predicate<string> filter)
    {
        IReadOnlyList<(string FirstName, string LastName)> emails = _sampleData.FilterByEmailAddress(filter).ToList();
        return Task.FromResult(emails);
    }

    protected override Task<string> GetAggregateSortedListOfStatesUsingCsvRows()
    {
        string s = _sampleData.GetAggregateSortedListOfStatesUsingCsvRows();
        return Task.FromResult(s);
    }

    protected override Task<IReadOnlyList<string>> GetUniqueSortedListOfStatesGivenCsvRows()
    {
        IReadOnlyList<string> states = _sampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToList();
        return Task.FromResult(states);
    }

    protected override Task<IReadOnlyList<IPerson>> People()
    {
        IReadOnlyList<IPerson> people = _sampleData.People.ToList();
        return Task.FromResult(people);
    }
}
