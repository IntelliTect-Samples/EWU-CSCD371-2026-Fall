using Microsoft.VisualStudio.TestTools.UnitTesting;
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

    //TODO: implement
    //[TestMethod]
    //public void TestGetUniqueSortedListOfStatesGivenCsvRowsHardcodedList()
    //{

    //}

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
