using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    public void CsvRows_SkipsHeader_Returns50Rows()
    {
        SampleData data = new();

        Assert.AreEqual<int>(50, data.CsvRows.Count());
        Assert.IsFalse(data.CsvRows.Any(row => row.StartsWith("Id,", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void CsvRows_FirstRow_IsFirstPerson()
    {
        SampleData data = new();

        Assert.AreEqual<string>(
            "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,MT,70577",
            data.CsvRows.First());
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_MatchesHardcodedList()
    {
        SampleData data = new();
        string[] expected =
        [
            "AL", "AZ", "CA", "DC", "FL", "GA", "IN", "KS", "LA", "MD",
            "MN", "MO", "MT", "NC", "NE", "NH", "NV", "NY", "OR", "PA",
            "SC", "TN", "TX", "UT", "VA", "WA", "WV"
        ];

        string[] actual = data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual).All(pair => pair.First == pair.Second));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_IsSortedAndUnique()
    {
        SampleData data = new();
        string[] states = data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        Assert.IsTrue(states.Zip(states.Skip(1))
            .All(pair => string.CompareOrdinal(pair.First, pair.Second) < 0));
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_MatchesHardcodedString()
    {
        SampleData data = new();

        Assert.AreEqual<string>(
            "AL, AZ, CA, DC, FL, GA, IN, KS, LA, MD, MN, MO, MT, NC, NE, NH, NV, NY, OR, PA, SC, TN, TX, UT, VA, WA, WV",
            data.GetAggregateSortedListOfStatesUsingCsvRows());
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_MatchesUniqueSortedList()
    {
        SampleData data = new();
        string[] expected = data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        string[] actual = data.GetAggregateSortedListOfStatesUsingCsvRows().Split(", ");

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual).All(pair => pair.First == pair.Second));
    }

}
