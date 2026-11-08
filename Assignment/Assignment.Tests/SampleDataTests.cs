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

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_ReturnsUniqueCommaSeparatedStates()
    {
        var sampleData = new SampleData();
        string[] states = sampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        string aggregatedStates = sampleData.GetAggregateSortedListOfStatesUsingCsvRows();

        Assert.AreEqual(string.Join(",", states), aggregatedStates);
        Assert.AreEqual(states.Length, aggregatedStates.Split(',').Distinct(StringComparer.Ordinal).Count());
    }

    [TestMethod]
    public void People_ReturnsAllCsvRowsAsSortedPeopleWithAddresses()
    {
        var sampleData = new SampleData();
        string[][] expectedRows = sampleData.CsvRows
            .Select(row => row.Split(','))
            .OrderBy(columns => columns[6], StringComparer.Ordinal)
            .ThenBy(columns => columns[5], StringComparer.Ordinal)
            .ThenBy(columns => columns[7], StringComparer.Ordinal)
            .ToArray();

        IPerson[] people = sampleData.People.ToArray();

        Assert.AreEqual(expectedRows.Length, people.Length);
        Assert.IsTrue(expectedRows
            .Zip(people, (columns, person) =>
                columns[1] == person.FirstName
                && columns[2] == person.LastName
                && columns[3] == person.EmailAddress
                && columns[4] == person.Address.StreetAddress
                && columns[5] == person.Address.City
                && columns[6] == person.Address.State
                && columns[7] == person.Address.Zip)
            .All(matches => matches));
    }
}
