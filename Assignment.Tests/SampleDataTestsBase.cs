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
    public async Task GetUniqueSortedListOfStatesGivenCsvRowsContainsStatesFromHardcodedAddresses()
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
            state => actualStates.Contains(state, StringComparer.Ordinal)),
            "GetUniqueSortedListOfStatesGivenCsvRowsAsync should include every state from the hardcoded CSV addresses.");
    }

    [TestMethod]
    public async Task GetUniqueSortedListOfStatesGivenCsvRowsReturnsUniqueAlphabeticallySortedStates()
    {
        string[] states = await GetUniqueSortedListOfStatesGivenCsvRowsAsync();

        Assert.AreEqual<int>(states.Length, states.Distinct(StringComparer.Ordinal).Count(),
            "GetUniqueSortedListOfStatesGivenCsvRowsAsync must return each state exactly once.");
        Assert.IsTrue(states
            .Zip(states.Skip(1), (current, next) => string.CompareOrdinal(current, next) <= 0)
            .All(isSorted => isSorted),
            "GetUniqueSortedListOfStatesGivenCsvRowsAsync must return states in alphabetical order.");
    }

    [TestMethod]
    public async Task GetAggregateSortedListOfStatesUsingCsvRowsReturnsUniqueCommaSeparatedStates()
    {
        string[] states = await GetUniqueSortedListOfStatesGivenCsvRowsAsync();

        string aggregatedStates = await GetAggregateSortedListOfStatesUsingCsvRowsAsync();

        Assert.AreEqual<string>(string.Join(",", states), aggregatedStates,
            "GetAggregateSortedListOfStatesUsingCsvRowsAsync should match the unique, alphabetized state list from the CSV data.");
        Assert.AreEqual<int>(states.Length, aggregatedStates.Split(',').Distinct(StringComparer.Ordinal).Count(),
            "GetAggregateSortedListOfStatesUsingCsvRowsAsync must produce a comma-separated list with no duplicate states.");
    }

    [TestMethod]
    public async Task PeopleReturnsAllCsvRowsAsSortedPeopleWithAddresses()
    {
        string[][] expectedRows = (await GetCsvRowsAsync())
            .Select(row => row.Split(','))
            .OrderBy(columns => columns[6], StringComparer.Ordinal)
            .ThenBy(columns => columns[5], StringComparer.Ordinal)
            .ThenBy(columns => columns[7], StringComparer.Ordinal)
            .ToArray();

        IPerson[] people = await GetPeopleAsync();

        Assert.AreEqual<int>(expectedRows.Length, people.Length,
            "GetPeopleAsync should return one Person per CSV row after sorting by state, city, and zip.");
        Assert.IsTrue(expectedRows
            .Zip(people, (columns, person) =>
                columns[1] == person.FirstName
                && columns[2] == person.LastName
                && columns[3] == person.EmailAddress
                && columns[4] == person.Address.StreetAddress
                && columns[5] == person.Address.City
                && columns[6] == person.Address.State
                && columns[7] == person.Address.Zip)
            .All(matches => matches),
            "Each Person should match the corresponding CSV row values and address fields in sorted order.");
    }

    [TestMethod]
    public async Task FilterByEmailAddressGovDomainReturnsHardcodedNamesInPeopleOrder()
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

        Assert.AreEqual<int>(expected.Length, actual.Length,
            "FilterByEmailAddressAsync should return exactly the 4 expected .gov matches in the People dataset.");
        Assert.IsTrue(expected.Zip(actual, (left, right) => left == right).All(matches => matches),
            "FilterByEmailAddressAsync should return the expected .gov names in the same order as the original People collection.");
    }

    [TestMethod]
    public async Task FilterByEmailAddressMatchesPeopleFilteredWithLinq()
    {
        static bool IsEduEmail(string email) => email.Contains("edu", StringComparison.Ordinal);

        (string FirstName, string LastName)[] expected = (await GetPeopleAsync())
            .Where(person => IsEduEmail(person.EmailAddress))
            .Select(person => (person.FirstName, person.LastName))
            .ToArray();

        (string FirstName, string LastName)[] actual = await FilterByEmailAddressAsync(IsEduEmail);

        Assert.IsTrue(expected.Length > 0,
            "Expected at least one email address containing 'edu' in the People dataset.");
        Assert.AreEqual<int>(expected.Length, actual.Length,
            "FilterByEmailAddressAsync should return the same number of people whose email addresses contain 'edu' as the LINQ-based expectation.");
        Assert.IsTrue(expected.Zip(actual, (left, right) => left == right).All(matches => matches),
            "FilterByEmailAddressAsync should match the same names as the LINQ filter for emails containing 'edu'.");
    }

    [TestMethod]
    public async Task FilterByEmailAddressExactEmailReturnsSinglePerson()
    {
        (string FirstName, string LastName)[] actual =
            await FilterByEmailAddressAsync(email => email == "kjoder1@quantcast.com");

        Assert.AreEqual<int>(1, actual.Length,
            "FilterByEmailAddressAsync should return exactly one person when the exact email address is specified.");
        Assert.AreEqual<(string, string)>(("Karin", "Joder"), actual[0],
            "FilterByEmailAddressAsync should return the matching person for the email 'kjoder1@quantcast.com'.");
    }

    [TestMethod]
    public async Task FilterByEmailAddressNoMatchesReturnsEmpty()
    {
        (string FirstName, string LastName)[] actual =
            await FilterByEmailAddressAsync(email => email.EndsWith(".invalid", StringComparison.Ordinal));

        Assert.AreEqual<int>(0, actual.Length,
            "FilterByEmailAddressAsync should return no results when no email ends with the invalid domain suffix.");
    }

    [TestMethod]
    public async Task FilterByEmailAddressNullFilterThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => FilterByEmailAddressAsync(null!),
            "FilterByEmailAddressAsync should throw ArgumentNullException when the email filter is null.");
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollectionMatchesUniqueSortedStatesFromCsvRows()
    {
        string expected = string.Join(",", await GetUniqueSortedListOfStatesGivenCsvRowsAsync());

        string actual = await GetAggregateListOfStatesGivenPeopleCollectionAsync(await GetPeopleAsync());

        Assert.AreEqual<string>(expected, actual,
            "GetAggregateListOfStatesGivenPeopleCollectionAsync should match the unique sorted CSV states derived from the same people data.");
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollectionDuplicateStatesReturnsUniqueSortedList()
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

        Assert.AreEqual<string>("AZ,CA,WA", actual,
            "GetAggregateListOfStatesGivenPeopleCollectionAsync should deduplicate and sort the provided states as AZ,CA,WA.");
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollectionEmptyCollectionReturnsEmptyString()
    {
        string actual = await GetAggregateListOfStatesGivenPeopleCollectionAsync([]);

        Assert.AreEqual<string>(string.Empty, actual,
            "GetAggregateListOfStatesGivenPeopleCollectionAsync should return an empty string when the collection is empty.");
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollectionNullPeopleThrowsArgumentNullException()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(
            () => GetAggregateListOfStatesGivenPeopleCollectionAsync(null!),
            "GetAggregateListOfStatesGivenPeopleCollectionAsync should throw ArgumentNullException when people is null.");
    }

    private static Person CreatePerson(string state)
        => new("First", "Last", "first.last@example.com",
            new Address("1 Main St", "City", state, "00000"));
}