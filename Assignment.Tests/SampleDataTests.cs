using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    public void CsvRows_ConstructedProperly_EnumeratesProperly()
    {
        // Arrange
        SampleData sampleData = new();
        // Act
        var csvRows = sampleData.CsvRows;
        // Assert
        Assert.IsNotNull(csvRows);
        foreach (string row in csvRows)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(row));
            Assert.IsInstanceOfType<string>(row);
        }
    }

    [TestMethod]
    public void CsvRows_StreamsAnd_SkipsHeader()
    {
        // Arrange
        SampleData sampleData = new();
        // Act
        var csvRows = sampleData.CsvRows.ToArray();
        // Assert
        Assert.AreNotEqual(0, csvRows.Length);
        Assert.IsFalse(csvRows[0].StartsWith("Id,", StringComparison.Ordinal));
        Assert.IsTrue(csvRows.All(r => !string.IsNullOrWhiteSpace(r)));
    }

    [TestMethod]
    public void StatesFromCSV_AreUniqueAndSorted_UsingLINQChecks()
    {
        // Arrange
        SampleData sampleData = new();
        var states = sampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();
        // Act
        var nonDecreasing = states.Zip(states.Skip(1), (a, b) => string.Compare(a, b, StringComparison.OrdinalIgnoreCase) <= 0);
        // Assert
        Assert.IsTrue(nonDecreasing.All(x => x));
        Assert.AreEqual(states.Length, states.Distinct(StringComparer.OrdinalIgnoreCase).Count());
    }

    [TestMethod]
    public void States_HardcodedAddresses_UniqueAndSorted()
    {
        // Arrange
        var hardcodedRows = new[]
        {
            "0,A,B,a@x, 123 Main, City, TX, 00001",
            "0,A,B,b@x, 123 Main, City, WA, 00001",
            "0,A,B,c@x, 123 Main, City, CA, 00001",
            "0,A,B,d@x, 123 Main, City, TX, 00001",
            "0,A,B,e@x, 123 Main, City, ca, 00001"
        };

        var actual = hardcodedRows
            .Select(line => line.Split(',', StringSplitOptions.TrimEntries)[6])
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var expected = new[] { "CA", "TX", "WA" };
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void AggregateStates_FromCsvEquals_JoinOfUniqueList()
    {
        var sd = new SampleData();
        var uniqueList = sd.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();
        var aggregate = sd.GetAggregateSortedListOfStatesUsingCsvRows();

        Assert.AreEqual(string.Join(", ", uniqueList), aggregate);
    }

    [TestMethod]
    public void People_MapAllColumnsAndSortByStateCityZipCorrectly_Success()
    {
        // Arrange
        SampleData sampleData = new();
        var people = sampleData.People.ToArray();

        // Act & Assert
        Assert.IsGreaterThan(0, people.Length);
        foreach (var person in people)
        {
            Assert.IsNotNull(person);
            Assert.IsInstanceOfType<IPerson>(person);
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.FirstName));
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.LastName));
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.EmailAddress));
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.Address.City));
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.Address.State));
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.Address.Zip));
        }

        var reSorted = people
            .OrderBy(p => p.Address.State, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Address.City, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Address.Zip, StringComparer.OrdinalIgnoreCase)
            .ToArray();
        CollectionAssert.AreEqual(reSorted, people);
    }

    [TestMethod]
    public void FilterByEmailAddress_Returns_CorrectTuples()
    {
        // Arrange
        SampleData sampleData = new();
        var actual = sampleData.FilterByEmailAddress(email => email.EndsWith(".edu", StringComparison.OrdinalIgnoreCase)).ToArray();
        // Act
        var expected = sampleData.People
            .Where(p => p.EmailAddress.EndsWith(".edu", StringComparison.OrdinalIgnoreCase))
            .Select(p => (p.FirstName, p.LastName))
            .ToArray();
        // Assert
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void AggregateStatesFromPeople_UsesDistinctAndAggregate_Success()
    {
        // Arrange
        SampleData sampleData = new();
        // Act
        var expected = string.Join(", ",
            sampleData.People
                .Select(p => p.Address.State)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(s => s, StringComparer.OrdinalIgnoreCase)
        );

        var actual = sampleData.GetAggregateListOfStatesGivenPeopleCollection(sampleData.People);
        // Assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void AggregateStatesFromPeople_EmptyInput_ReturnsEmptyString()
    {
        // Arrange
        SampleData sampleData = new();
        // Act
        var actual = sampleData.GetAggregateListOfStatesGivenPeopleCollection(Enumerable.Empty<IPerson>());
        // Assert
        Assert.AreEqual(string.Empty, actual);
    }

}
