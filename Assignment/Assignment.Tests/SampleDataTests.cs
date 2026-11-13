using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    public void CsvRows_SkipsHeaderAndReturnsAllPeople()
    {
        // Arrange
        var data = new SampleData();

        // Act
        var rows = data.CsvRows.ToList();

        // Assert
        Assert.AreEqual(50, rows.Count);
        Assert.IsTrue(rows[0].StartsWith("1,Priscilla,", StringComparison.Ordinal));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStates_MatchesHardcodedExpectedStates()
    {
        // Arrange
        var data = new SampleData();

        string[] expectedStates =
        {
        "AL", "AZ", "CA", "DC", "FL", "GA",
        "IN", "KS", "LA", "MD", "MN", "MO",
        "MT", "NC", "NE", "NH", "NV", "NY",
        "OR", "PA", "SC", "TN", "TX", "UT",
        "VA", "WA", "WV"
        };

        // Act
        var states = data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        // Assert
        Assert.AreEqual(expectedStates.Length, states.Length);

        Assert.IsTrue(expectedStates.SequenceEqual(states));

        Assert.IsTrue(states.SequenceEqual(
            states.Distinct().OrderBy(state => state)));
    }

    [TestMethod]
    public void GetAggregateSortedListOfStates_VerifiesUniqueSortedStatesUsingLinq()
    {
        // Arrange
        var data = new SampleData();

        // Act
        var result = data.GetAggregateSortedListOfStatesUsingCsvRows();

        var states = result.Split(", ", StringSplitOptions.RemoveEmptyEntries);

        // Assert
        Assert.IsTrue(states.Length > 0);
        Assert.AreEqual(states.Length, states.Distinct().Count());

        Assert.IsTrue(states.SequenceEqual(
        states.OrderBy(state => state)));
    }

    [TestMethod]
    public void People_ReturnsSortedPeopleWithAddresses()
    {
        // Arrange
        var data = new SampleData();

        var expected = data.CsvRows
            .Select(row => row.Split(','))
            .OrderBy(columns => columns[6])
            .ThenBy(columns => columns[5])
            .ThenBy(columns => columns[7])
            .ToArray();

        // Act
        var people = data.People.ToArray();

        // Assert
        Assert.AreEqual(50, people.Length);
        Assert.AreEqual(expected.Length, people.Length);

        for (int i = 0; i < people.Length; i++)
        {
            Assert.AreEqual(expected[i][1], people[i].FirstName);
            Assert.AreEqual(expected[i][2], people[i].LastName);
            Assert.AreEqual(expected[i][3], people[i].EmailAddress);
            Assert.AreEqual(expected[i][4], people[i].Address.StreetAddress);
            Assert.AreEqual(expected[i][5], people[i].Address.City);
            Assert.AreEqual(expected[i][6], people[i].Address.State);
            Assert.AreEqual(expected[i][7], people[i].Address.Zip);
        }
    }

    [TestMethod]
    public void FilterByEmailAddress_ReturnsMatchingPeople()
    {
        // Arrange
        var data = new SampleData();

        Predicate<string> filter = email =>
            email.EndsWith(".gov", StringComparison.OrdinalIgnoreCase);

        var expected = data.People
            .Where(person => filter(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName))
            .ToArray();

        // Act
        var results = data.FilterByEmailAddress(filter).ToArray();

        // Assert
        Assert.IsTrue(results.Length > 0);
        Assert.IsTrue(expected.SequenceEqual(results));
    }

    [TestMethod]
    public void GetAggregateListOfStates_ReturnsUniqueSortedStates()
    {
        // Arrange
        var data = new SampleData();

        var expected = string.Join(", ",
            data.GetUniqueSortedListOfStatesGivenCsvRows());

        // Act
        var result = data.GetAggregateListOfStatesGivenPeopleCollection(data.People);

        // Assert
        Assert.AreEqual(expected, result);
    }

    [TestMethod]
    public void GetAggregateListOfStates_UsesProvidedPeople()
    {
        // Arrange
        var data = new SampleData();

        var people = data.People
            .Where(person => person.Address.State == "WA"
                || person.Address.State == "CA")
            .ToArray();

        // Act
        var result = data.GetAggregateListOfStatesGivenPeopleCollection(people);

        // Assert
        Assert.AreEqual("CA, WA", result);
    }

    [TestMethod]
    public void GetAggregateListOfStates_EmptyCollectionReturnsEmptyString()
    {
        // Arrange
        var data = new SampleData();
        var people = Array.Empty<IPerson>();

        // Act
        var result = data.GetAggregateListOfStatesGivenPeopleCollection(people);

        // Assert
        Assert.AreEqual(string.Empty, result);
    }

    [TestMethod]
    public void FilterByEmailAddress_NullFilterThrowsException()
    {
        // Arrange
        var data = new SampleData();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(
            () => data.FilterByEmailAddress(null!));
    }

    [TestMethod]
    public void FilterByEmailAddress_NoMatchesReturnsEmpty()
    {
        // Arrange
        var data = new SampleData();

        // Act
        var results = data.FilterByEmailAddress(
            email => email.EndsWith("@notfound.example",
                StringComparison.OrdinalIgnoreCase)).ToArray();

        // Assert
        Assert.AreEqual(0, results.Length);
    }

    [TestMethod]
    public void GetAggregateListOfStates_NullPeopleThrowsException()
    {
        // Arrange
        var data = new SampleData();

        // Act and Assert
        Assert.ThrowsExactly<ArgumentNullException>(
            () => data.GetAggregateListOfStatesGivenPeopleCollection(null!));
    }
}
