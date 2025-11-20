using Assignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    // A derived class to override CsvRows for testing purposes
    private sealed class SampleDataForTest : SampleData
    {
        private readonly IEnumerable<string> _rows;

        public override IEnumerable<string> CsvRows => _rows;

        public SampleDataForTest()
        {
            _rows = [
                "1,Alice,Smith,alice@email.com,123 Main St,Seattle,WA,98101",
                "2,Bob,Lee,bob@email.com,456 Elm St,San Francisco,CA,94102",
                "3,Charlie,Brown,charlie@email.com,789 Oak St,Los Angeles,CA,90001",
                "4,Dave,Clark,dave@gmail.com,111 Pine St,Portland,OR,97201"
            ];
        }
    }

    [TestMethod]
    public void CsvRows_ValidCsvRows_LoadsRowsAndSkipsHeader()
    {
        // Arrange & Act
        var data = new SampleData();
        var rows = data.CsvRows.ToList();

        // Assert
        Assert.IsNotEmpty(rows);
        Assert.IsFalse(rows.Any(r => r.Contains("FirstName")));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_AddressesWithDuplicateStates_ReturnsStatesWithoutDuplicates()
    {
        // Arrange
        var data = new SampleDataForTest();

        // Act
        var result = data.GetUniqueSortedListOfStatesGivenCsvRows().ToList();

        // Assert
        CollectionAssert.AreEqual(new List<string> { "CA", "OR", "WA" }, result);
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_UnsortedAddresses_ReturnsSortedStates()
    {
        // Arrange
        var data = new SampleDataForTest();

        // Act
        var result = data.GetUniqueSortedListOfStatesGivenCsvRows().ToList();

        // Assert
        var sorted = result.OrderBy(s => s).ToList();
        Assert.IsTrue(result.SequenceEqual(sorted));
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_UnsortedAddressesWithDuplicates_ReturnsSortedAggregateString()
    {
        // Arrange
        var data = new SampleDataForTest();

        var result = data.GetAggregateSortedListOfStatesUsingCsvRows();
        Assert.AreEqual<string>("CA,OR,WA", result);
    }

    [TestMethod]
    public void People_ValidCsvRows_LoadsPeopleOrderedByStateCityZip()
    {
        // Arrange & Act
        var data = new SampleData();
        var people = data.People.ToList();

        var sorted = people.OrderBy(person => person.Address.State).ThenBy(person => person.Address.City).ThenBy(person => person.Address.Zip).ToList();

        Assert.HasCount(data.CsvRows.Count(), people);
        Assert.IsTrue(people.SequenceEqual(sorted));
    }

    [TestMethod]
    public void FilterByEmailAddress_ValidAddresses_ReturnsMatchingPeoplesNames()
    {
        // Arrange
        var data = new SampleDataForTest();

        // Act
        var result = data.FilterByEmailAddress(email => email.EndsWith("gmail.com", StringComparison.InvariantCulture));

        // Assert
        Assert.HasCount(1, result);
        Assert.IsTrue(result.Contains(("Dave", "Clark")));
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_PeopleWithDuplicateStates_ReturnsSortedAggregateString()
    {
        // Arrange
        var data = new SampleDataForTest();
        var people = data.People;

        // Act
        var result = data.GetAggregateListOfStatesGivenPeopleCollection(people);

        // Assert
        Assert.AreEqual<string>(data.GetAggregateSortedListOfStatesUsingCsvRows(), result);
    }
}
