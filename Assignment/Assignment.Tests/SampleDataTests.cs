using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;



namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    private readonly ISampleData _sampleData =
        new SampleData();

    [TestMethod]
    public void TestCsvRowsSkipsHeader()
    {
        Assert.DoesNotContain("FirstName", _sampleData.CsvRows.First());
    }

    [TestMethod]
    public void TestGetUniqueSortedListOfStatesGivenCsvRowsHardcodedList()
    {
    List<string> states = [
        "CA",
        "WA",
        "CA",
        "TX",
        "WA"];

    List<string> expected =
    [
        "CA",
        "TX",
        "WA"
    ];

    Assert.AreSequenceEqual(expected,states.Distinct()
            .OrderBy(s => s)
            .ToList());
    }
    [TestMethod]
    public void TestPeopleCountMatchesCsvRows()
    {
        Assert.AreEqual<int>(
            _sampleData.CsvRows.Count(),
            _sampleData.People.Count());
    }
    [TestMethod]
    public void TestPeopleHaveAddresses()
    {
        Assert.IsTrue(
            _sampleData.People.All(
                p => p.Address != null));
    }
    [TestMethod]
    public void TestFilterByEmailAddress()
    {
        var result =
            _sampleData.FilterByEmailAddress(
                email => email.EndsWith(".edu"));

        Assert.IsTrue(result.Any());
    }
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
    [TestMethod]
    public void TestGetUniqueSortedListOfStatesGivenCsvRowsWithLinq()
    {
        var rows = _sampleData.CsvRows.Select(row => row.Split(',')[6]).Distinct().OrderBy(State => State).ToList();
        Assert.AreSequenceEqual(rows.Distinct().OrderBy(state => state), _sampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToList());
    }

    [TestMethod]
    public void TestGetAggregateSortedListOfStatesUsingCsvRows()
    {
        string stringList = _sampleData.GetUniqueSortedListOfStatesGivenCsvRows().Distinct().OrderBy(State => State).Aggregate((a, b) => a + ", " + b);
        Assert.AreEqual(stringList, _sampleData.GetAggregateSortedListOfStatesUsingCsvRows());
    }
}
