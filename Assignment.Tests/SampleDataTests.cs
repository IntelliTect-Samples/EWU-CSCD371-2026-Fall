using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    private static readonly string[] expected = ["OR", "WA"];

    [TestMethod]
    public void GetUniqueSortedListOfStates_HardcodedDuplicateStates_ReturnsUniqueSorted()
    {
        string[] csvRows =
        [
            "1,John,Doe,john@test.com,123 Main,Seattle,WA,98101",
            "2,Jane,Smith,jane@test.com,456 Oak,Portland,OR,97201",
            "3,Bob,Jones,bob@test.com,789 Pine,Spokane,WA,99201",
            "4,Alice,Brown,alice@test.com,321 Elm,Eugene,OR,97401"
        ];
        IEnumerable<string> result = SampleData.GetUniqueSortedListOfStates(csvRows);
        string[] states = result.ToArray();
        Assert.HasCount(2, states);
        Assert.IsTrue(states.Zip(expected, (a, b) => a == b).All(match => match));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_ActualData_VerifiesSortedAndUnique()
    {
        SampleData sample = new();
        string[] states = sample.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();
        string[] sortedStates = states.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToArray();
        Assert.IsTrue(states.Zip(sortedStates, (a, b) => a == b).All(match => match));
        int distinctCount = states.Distinct(StringComparer.OrdinalIgnoreCase).Count();
        Assert.HasCount(distinctCount, states);
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_ActualData_ReturnsCommaSeparatedString()
    {
        SampleData sample = new();
        string result = sample.GetAggregateSortedListOfStatesUsingCsvRows();
        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        Assert.Contains(", ", result);
        string[] states = result.Split(", ", StringSplitOptions.RemoveEmptyEntries);
        string[] sortedStates = states.OrderBy(s => s, StringComparer.OrdinalIgnoreCase).ToArray();
        Assert.IsTrue(states.Zip(sortedStates, (a, b) => a == b).All(match => match));
    }

    [TestMethod]
    public void People_ActualData_ReturnsSortedByStateAndCityAndZip()
    {
        SampleData sample = new();
        IPerson[] people = sample.People.ToArray();
        Assert.IsNotEmpty(people);
        Assert.IsTrue(people.All(p => p.Address != null));
        IPerson[] manualSort = people
            .OrderBy(p => p.Address.State, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Address.City, StringComparer.OrdinalIgnoreCase)
            .ThenBy(p => p.Address.Zip, StringComparer.OrdinalIgnoreCase).ToArray();

        Assert.IsTrue(people.Zip(manualSort, (a, b) => ReferenceEquals(a, b)).All(match => match));
    }

    [TestMethod]
    public void People_ActualData_AddressPropertiesPopulated()
    {
        SampleData sample = new();
        IPerson[] people = sample.People.ToArray();

        foreach (IPerson person in people)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.FirstName));
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.LastName));
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.EmailAddress));
            Assert.IsNotNull(person.Address);
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.Address.StreetAddress));
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.Address.City));
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.Address.State));
            Assert.IsFalse(string.IsNullOrWhiteSpace(person.Address.Zip));
        }
    }

    [TestMethod]
    public void FilterByEmailAddress_StanfordEmail_ReturnsOnlyMatchingNames()
    {
        SampleData sample = new();
        IEnumerable<(string FirstName, string LastName)> result =
            sample.FilterByEmailAddress(email => email.EndsWith("@stanford.edu", StringComparison.OrdinalIgnoreCase));
        (string FirstName, string LastName)[] matches = result.ToArray();
        IPerson[] allPeople = sample.People.ToArray();
        IPerson[] expectedMatches = allPeople.Where(p => p.EmailAddress.EndsWith("@stanford.edu", StringComparison.OrdinalIgnoreCase)).ToArray();
        Assert.HasCount(expectedMatches.Length, matches);

        foreach ((string FirstName, string LastName) in matches)
        {
            Assert.IsTrue(expectedMatches.Any(p => p.FirstName == FirstName && p.LastName == LastName));
        }
    }

    [TestMethod]
    public void FilterByEmailAddress_EmailContainsGov_ReturnsMatchingNames()
    {
        SampleData sample = new();
        IEnumerable<(string FirstName, string LastName)> result =
            sample.FilterByEmailAddress(email => email.Contains(".gov", StringComparison.OrdinalIgnoreCase));

        (string FirstName, string LastName)[] matches = result.ToArray();
        Assert.IsNotEmpty(matches);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_ActualPeople_ReturnsUniqueCommaSeparated()
    {
        SampleData sample = new();
        IPerson[] people = sample.People.ToArray();
        string result = sample.GetAggregateListOfStatesGivenPeopleCollection(people);
        Assert.IsFalse(string.IsNullOrWhiteSpace(result));
        string[] states = result.Split(", ", StringSplitOptions.RemoveEmptyEntries);
        int distinctCount = states.Distinct(StringComparer.OrdinalIgnoreCase).Count();
        Assert.HasCount(distinctCount, states);

        string[] expectedStates = sample.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();
        Assert.IsTrue(states.All(s => expectedStates.Contains(s, StringComparer.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_OrderedInput_MaintainsOrder()
    {
        List<IPerson> testPeople =
        [
            new Person("A", "A", new Address("1", "City1", "WA", "1"), "a@test.com"),
            new Person("B", "B", new Address("2", "City2", "OR", "2"), "b@test.com"),
            new Person("C", "C", new Address("3", "City3", "CA", "3"), "c@test.com"),
            new Person("D", "D", new Address("4", "City4", "WA", "4"), "d@test.com")
        ];

        SampleData sample = new();
        string result = sample.GetAggregateListOfStatesGivenPeopleCollection(testPeople);
        Assert.AreEqual<string>("CA, OR, WA", result);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_ActualPeople_MatchesGetUniqueSortedListOfStatesGivenCsvRows()
    {
        SampleData sample = new();
        string result = sample.GetAggregateListOfStatesGivenPeopleCollection(sample.People);
        string expected = sample.GetAggregateSortedListOfStatesUsingCsvRows();
        Assert.AreEqual(expected, result);
    }
}
