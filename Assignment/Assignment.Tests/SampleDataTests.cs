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

    [TestMethod]
    public void FilterByEmailAddress_GovDomain_ReturnsHardcodedNamesInPeopleOrder()
    {
        (string FirstName, string LastName)[] expected =
        [
            ("Ev", "Challace"),       // CA
            ("Marijn", "McKennan"),   // DC
            ("Priscilla", "Jenyns"),  // MT
            ("Amelia", "Toal")        // WV
        ];

        (string FirstName, string LastName)[] actual = new SampleData()
            .FilterByEmailAddress(email => email.EndsWith(".gov", StringComparison.Ordinal))
            .ToArray();

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual, (left, right) => left == right).All(matches => matches));
    }

    [TestMethod]
    public void FilterByEmailAddress_MatchesPeopleFilteredWithLinq()
    {
        var sampleData = new SampleData();
        static bool IsEduEmail(string email) => email.Contains("edu", StringComparison.Ordinal);

        (string FirstName, string LastName)[] expected = sampleData.People
            .Where(person => IsEduEmail(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName))
            .ToArray();

        (string FirstName, string LastName)[] actual = sampleData.FilterByEmailAddress(IsEduEmail).ToArray();

        Assert.IsTrue(expected.Length > 0);
        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual, (left, right) => left == right).All(matches => matches));
    }

    [TestMethod]
    public void FilterByEmailAddress_ExactEmail_ReturnsSinglePerson()
    {
        (string FirstName, string LastName)[] actual = new SampleData()
            .FilterByEmailAddress(email => email == "kjoder1@quantcast.com")
            .ToArray();

        Assert.AreEqual<int>(1, actual.Length);
        Assert.AreEqual<(string, string)>(("Karin", "Joder"), actual[0]);
    }

    [TestMethod]
    public void FilterByEmailAddress_NoMatches_ReturnsEmpty()
    {
        int count = new SampleData()
            .FilterByEmailAddress(email => email.EndsWith(".invalid", StringComparison.Ordinal))
            .Count();

        Assert.AreEqual<int>(0, count);
    }

    [TestMethod]
    public void FilterByEmailAddress_NullFilter_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new SampleData().FilterByEmailAddress(null!));
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_MatchesUniqueSortedStatesFromCsvRows()
    {
        var sampleData = new SampleData();
        string expected = string.Join(",", sampleData.GetUniqueSortedListOfStatesGivenCsvRows());

        string actual = sampleData.GetAggregateListOfStatesGivenPeopleCollection(sampleData.People);

        Assert.AreEqual<string>(expected, actual);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_DuplicateStates_ReturnsUniqueSortedList()
    {
        IPerson[] people =
        [
            CreatePerson("WA"),
            CreatePerson("CA"),
            CreatePerson("WA"),
            CreatePerson("AZ"),
            CreatePerson("CA")
        ];

        string actual = new SampleData().GetAggregateListOfStatesGivenPeopleCollection(people);

        Assert.AreEqual<string>("AZ,CA,WA", actual);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_EmptyCollection_ReturnsEmptyString()
    {
        string actual = new SampleData().GetAggregateListOfStatesGivenPeopleCollection([]);

        Assert.AreEqual<string>(string.Empty, actual);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_NullPeople_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new SampleData().GetAggregateListOfStatesGivenPeopleCollection(null!));
    }

    private static Person CreatePerson(string state)
        => new("First", "Last", "first.last@example.com",
            new Address("1 Main St", "City", state, "00000"));
}