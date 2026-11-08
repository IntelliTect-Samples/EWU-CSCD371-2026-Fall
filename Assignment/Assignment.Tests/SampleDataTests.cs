using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_ContainsStatesFromHardcodedAddresses()
    {
        string[] hardcodedAddresses =
        [
            "7884 Corry Way,Helena,MT,70577",
            "03594 Florence Park,Tampa,FL,71961",
            "94148 Kings Terrace,Long Beach,CA,59721"
        ];
        string[] expectedStates = hardcodedAddresses
            .Select(address => address.Split(',', StringSplitOptions.TrimEntries)[2])
            .Distinct(StringComparer.Ordinal)
            .ToArray();

        string[] actualStates = new SampleData()
            .GetUniqueSortedListOfStatesGivenCsvRows()
            .ToArray();

        Assert.IsTrue(expectedStates.All(
            state => actualStates.Contains(state, StringComparer.Ordinal)));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_ReturnsUniqueAlphabeticallySortedStates()
    {
        string[] states = new SampleData()
            .GetUniqueSortedListOfStatesGivenCsvRows()
            .ToArray();

        Assert.AreEqual(states.Length, states.Distinct(StringComparer.Ordinal).Count());
        Assert.IsTrue(states
            .Zip(states.Skip(1), (current, next) => string.CompareOrdinal(current, next) <= 0)
            .All(isSorted => isSorted));
    }
}
