using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

/// <summary>
/// Every test is written once here. SampleDataTests and SampleDataAsyncTests
/// each adapt their implementation to these awaitable, materialized results.
/// </summary>
public abstract class SampleDataTestsBase
{
    protected abstract Task<string[]> GetCsvRowsAsync();
    protected abstract Task<string[]> GetUniqueSortedListOfStatesGivenCsvRowsAsync();
    protected abstract Task<string> GetAggregateSortedListOfStatesUsingCsvRowsAsync();
    protected abstract Task<IPerson[]> GetPeopleAsync();
    protected abstract Task<(string FirstName, string LastName)[]> FilterByEmailAddressAsync(Predicate<string> filter);
    protected abstract Task<string> GetAggregateListOfStatesGivenPeopleCollectionAsync(IEnumerable<IPerson> people);

    [TestMethod]
    public async Task GetUniqueSortedListOfStatesGivenCsvRows_ContainsStatesFromHardcodedAddresses()
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

        string[] actualStates = await GetUniqueSortedListOfStatesGivenCsvRowsAsync();

        Assert.IsTrue(expectedStates.All(
            state => actualStates.Contains(state, StringComparer.Ordinal)));
    }

    [TestMethod]
    public async Task GetUniqueSortedListOfStatesGivenCsvRows_ReturnsUniqueAlphabeticallySortedStates()
    {
        string[] states = await GetUniqueSortedListOfStatesGivenCsvRowsAsync();

        Assert.AreEqual<int>(states.Length, states.Distinct(StringComparer.Ordinal).Count());
        Assert.IsTrue(states
            .Zip(states.Skip(1), (current, next) => string.CompareOrdinal(current, next) <= 0)
            .All(isSorted => isSorted));
    }

    [TestMethod]
    public async Task GetAggregateSortedListOfStatesUsingCsvRows_ReturnsUniqueCommaSeparatedStates()
    {
        string[] states = await GetUniqueSortedListOfStatesGivenCsvRowsAsync();

        string aggregatedStates = await GetAggregateSortedListOfStatesUsingCsvRowsAsync();

        Assert.AreEqual<string>(string.Join(",", states), aggregatedStates);
        Assert.AreEqual<int>(states.Length, aggregatedStates.Split(',').Distinct(StringComparer.Ordinal).Count());
    }

    [TestMethod]
    public async Task People_ReturnsAllCsvRowsAsSortedPeopleWithAddresses()
    {
        string[][] expectedRows = (await GetCsvRowsAsync())
            .Select(row => row.Split(','))
            .OrderBy(columns => columns[6], StringComparer.Ordinal)
            .ThenBy(columns => columns[5], StringComparer.Ordinal)
            .ThenBy(columns => columns[7], StringComparer.Ordinal)
            .ToArray();

        IPerson[] people = await GetPeopleAsync();

        Assert.AreEqual<int>(expectedRows.Length, people.Length);
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
    public async Task FilterByEmailAddress_GovDomain_ReturnsHardcodedNamesInPeopleOrder()
    {
        (string FirstName, string LastName)[] expected =
        [
            ("Ev", "Challace"),       // CA
            ("Marijn", "McKennan"),   // DC
            ("Priscilla", "Jenyns"),  // MT
            ("Amelia", "Toal")        // WV
        ];

        (string FirstName, string LastName)[] actual =
            await FilterByEmailAddressAsync(email => email.EndsWith(".gov", StringComparison.Ordinal));

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual, (left, right) => left == right).All(matches => matches));
    }

    [TestMethod]
    public async Task FilterByEmailAddress_MatchesPeopleFilteredWithLinq()
    {
        static bool IsEduEmail(string email) => email.Contains("edu", StringComparison.Ordinal);

        (string FirstName, string LastName)[] expected = (await GetPeopleAsync())
            .Where(person => IsEduEmail(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName))
            .ToArray();

        (string FirstName, string LastName)[] actual = await FilterByEmailAddressAsync(IsEduEmail);

        Assert.IsTrue(expected.Length > 0);
        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual, (left, right) => left == right).All(matches => matches));
    }

    [TestMethod]
    public async Task FilterByEmailAddress_ExactEmail_ReturnsSinglePerson()
    {
        (string FirstName, string LastName)[] actual =
            await FilterByEmailAddressAsync(email => email == "kjoder1@quantcast.com");

        Assert.AreEqual<int>(1, actual.Length);
        Assert.AreEqual<(string, string)>(("Karin", "Joder"), actual[0]);
    }

    [TestMethod]
    public async Task FilterByEmailAddress_NoMatches_ReturnsEmpty()
    {
        (string FirstName, string LastName)[] actual =
            await FilterByEmailAddressAsync(email => email.EndsWith(".invalid", StringComparison.Ordinal));

        Assert.AreEqual<int>(0, actual.Length);
    }

    [TestMethod]
    public async Task FilterByEmailAddress_NullFilter_ThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => FilterByEmailAddressAsync(null!));
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollection_MatchesUniqueSortedStatesFromCsvRows()
    {
        string expected = string.Join(",", await GetUniqueSortedListOfStatesGivenCsvRowsAsync());

        string actual = await GetAggregateListOfStatesGivenPeopleCollectionAsync(await GetPeopleAsync());

        Assert.AreEqual<string>(expected, actual);
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollection_DuplicateStates_ReturnsUniqueSortedList()
    {
        IPerson[] people =
        [
            CreatePerson("WA"),
            CreatePerson("CA"),
            CreatePerson("WA"),
            CreatePerson("AZ"),
            CreatePerson("CA")
        ];

        string actual = await GetAggregateListOfStatesGivenPeopleCollectionAsync(people);

        Assert.AreEqual<string>("AZ,CA,WA", actual);
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollection_EmptyCollection_ReturnsEmptyString()
    {
        string actual = await GetAggregateListOfStatesGivenPeopleCollectionAsync([]);

        Assert.AreEqual<string>(string.Empty, actual);
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollection_NullPeople_ThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => GetAggregateListOfStatesGivenPeopleCollectionAsync(null!));
    }

    private static Person CreatePerson(string state)
        => new("First", "Last", "first.last@example.com",
            new Address("1 Main St", "City", state, "00000"));
}