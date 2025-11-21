using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    public void CsvRows_ReturnCorrectNumberOfRows_Successfully()
    {
        // Arrange
        SampleData data = new SampleData();

        // Act
        var firstRow = data.CsvRows.First();
        var column = firstRow.Split(',');

        // Assert
        Assert.HasCount(8, column);
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_UsingLinq_Successfully()
    {
        // Arrange
        SampleData data = new SampleData();
        var expected = TestingPeopleData.Data.Skip(1)
            .Select(row => row.Split(',')[6])
            .Distinct()
            .OrderBy(state => state);

        // Act
        var actual = data.GetUniqueSortedListOfStatesGivenCsvRows();

        // Assert
        Assert.IsTrue(actual.SequenceEqual(expected));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_HardCodedList_Successfully()
    {
        // Arrange
        SampleData data = new SampleData();
        IEnumerable<string> expected = TestingPeopleData.uniqueStates.OrderBy(state => state);

        // Act
        IEnumerable<string> actual = data.GetUniqueSortedListOfStatesGivenCsvRows();

        // Assert
        Assert.IsTrue(actual.SequenceEqual(expected));

    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_ReturnsSortedStates_Successfully()
    {
        // Arrange
        SampleData data = new SampleData();
        string expected = string.Join(", ", data.GetUniqueSortedListOfStatesGivenCsvRows());

        // Act
        string actual = data.GetAggregateSortedListOfStatesUsingCsvRows();

        // Assert
        Assert.AreEqual<string>(expected, actual);
    }


    [TestMethod]
    public void People_ReturnsCorrectPerson_Successfully()
    {
        // Arrange
        SampleData data = new SampleData();

        // Act
        var people = data.People.ToList();
        var firstPerson = people.First();

        // Assert
        Assert.HasCount(data.CsvRows.Count(), people);
        Assert.IsFalse(string.IsNullOrWhiteSpace(firstPerson.FirstName));
        Assert.IsFalse(string.IsNullOrWhiteSpace(firstPerson.LastName));
        Assert.IsNotNull(firstPerson.Address);
        Assert.IsFalse(string.IsNullOrWhiteSpace(firstPerson.Address.State));
        Assert.IsFalse(string.IsNullOrWhiteSpace(firstPerson.Address.City));
    }

    [TestMethod]
    public void FilterByEmailAddress_ReturnsCorrectPerson_Successfully()
    {
        // Arrange
        SampleData data = new SampleData();
        string targetEmail = "pjenyns0@state.gov";

        Predicate<string> filter = item => item == targetEmail;

        // Act
        var result = data.FilterByEmailAddress(filter).Single();

        // Assert
        Assert.AreEqual<string>("Priscilla", result.FirstName);
        Assert.AreEqual<string>("Jenyns", result.LastName);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_SameResult_Successfully()
    {
        // Arrange
        SampleData data = new SampleData();
        string expected = string.Join(", ", data.GetUniqueSortedListOfStatesGivenCsvRows());
        
        // Act
        string actual = data.GetAggregateListOfStatesGivenPeopleCollection(data.People);

        // Assert
        Assert.AreEqual<string>(expected, actual);
    }
}