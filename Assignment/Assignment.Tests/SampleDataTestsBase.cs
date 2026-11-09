using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

public abstract class SampleDataTestsBase
{
    protected abstract Task<string[]> GetCsvRows();
    protected abstract Task<string[]> GetUniqueSortedStates();
    protected abstract Task<string> GetAggregateSortedStates();
    protected abstract Task<IPerson[]> GetPeople();
    protected abstract Task<(string FirstName, string LastName)[]> FilterByEmail(Predicate<string> filter);
    protected abstract Task<string> GetAggregateStatesFor(IPerson[]? people);

    [TestMethod]
    public async Task CsvRows_SkipsHeader_Returns50Rows()
    {
        string[] rows = await GetCsvRows();

        Assert.AreEqual<int>(50, rows.Length);
        Assert.IsFalse(rows.Any(row => row.StartsWith("Id,", StringComparison.Ordinal)));
    }

    [TestMethod]
    public async Task CsvRows_FirstRow_IsFirstPerson()
    {
        string[] rows = await GetCsvRows();

        Assert.AreEqual<string>(
            "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,MT,70577",
            rows.First());
    }

    [TestMethod]
    public async Task GetUniqueSortedListOfStatesGivenCsvRows_MatchesHardcodedList()
    {
        string[] expected =
        [
            "AL", "AZ", "CA", "DC", "FL", "GA", "IN", "KS", "LA", "MD",
            "MN", "MO", "MT", "NC", "NE", "NH", "NV", "NY", "OR", "PA",
            "SC", "TN", "TX", "UT", "VA", "WA", "WV"
        ];

        string[] actual = await GetUniqueSortedStates();

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual).All(pair => pair.First == pair.Second));
    }

    [TestMethod]
    public async Task GetUniqueSortedListOfStatesGivenCsvRows_IsSortedAndUnique()
    {
        string[] states = await GetUniqueSortedStates();

        Assert.IsTrue(states.Zip(states.Skip(1))
            .All(pair => string.CompareOrdinal(pair.First, pair.Second) < 0));
    }

    [TestMethod]
    public async Task GetAggregateSortedListOfStatesUsingCsvRows_MatchesHardcodedString()
    {
        Assert.AreEqual<string>(
            "AL, AZ, CA, DC, FL, GA, IN, KS, LA, MD, MN, MO, MT, NC, NE, NH, NV, NY, OR, PA, SC, TN, TX, UT, VA, WA, WV",
            await GetAggregateSortedStates());
    }

    [TestMethod]
    public async Task GetAggregateSortedListOfStatesUsingCsvRows_MatchesUniqueSortedList()
    {
        string[] expected = await GetUniqueSortedStates();

        string[] actual = (await GetAggregateSortedStates()).Split(", ");

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual).All(pair => pair.First == pair.Second));
    }

    [TestMethod]
    public async Task People_ReturnsOnePersonPerCsvRow()
    {
        Assert.AreEqual<int>((await GetCsvRows()).Length, (await GetPeople()).Length);
    }

    [TestMethod]
    public async Task People_AreSortedByStateThenCityThenZip()
    {
        IPerson[] people = await GetPeople();

        Assert.IsTrue(people.Zip(people.Skip(1)).All(pair =>
        {
            int state = string.CompareOrdinal(pair.First.Address.State, pair.Second.Address.State);
            int city = string.CompareOrdinal(pair.First.Address.City, pair.Second.Address.City);
            int zip = string.CompareOrdinal(pair.First.Address.Zip, pair.Second.Address.Zip);
            return state < 0 || (state == 0 && (city < 0 || (city == 0 && zip <= 0)));
        }));
    }

    [TestMethod]
    public async Task People_MatchCsvRowsIncludingAddress()
    {
        string[] expected = (await GetCsvRows())
            .Select(row => string.Join(',', row.Split(',').Skip(1)))
            .Order(StringComparer.Ordinal)
            .ToArray();
        string[] actual = (await GetPeople())
            .Select(person => string.Join(',',
                person.FirstName, person.LastName, person.EmailAddress,
                person.Address.StreetAddress, person.Address.City,
                person.Address.State, person.Address.Zip))
            .Order(StringComparer.Ordinal)
            .ToArray();

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual).All(pair => pair.First == pair.Second));
    }

    [TestMethod]
    public async Task People_FirstPerson_IsFromAlabama()
    {
        IPerson first = (await GetPeople()).First();

        Assert.AreEqual<string>("AL", first.Address.State);
    }

    [TestMethod]
    public async Task FilterByEmailAddress_GovEmails_ReturnsMatchingNamesInPeopleOrder()
    {
        (string FirstName, string LastName)[] expected =
        [
            ("Ev", "Challace"),
            ("Marijn", "McKennan"),
            ("Priscilla", "Jenyns"),
            ("Amelia", "Toal")
        ];

        (string FirstName, string LastName)[] actual =
            await FilterByEmail(email => email.EndsWith(".gov", StringComparison.Ordinal));

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual).All(pair => pair.First == pair.Second));
    }

    [TestMethod]
    public async Task FilterByEmailAddress_ExactEmail_ReturnsSinglePerson()
    {
        (string FirstName, string LastName)[] actual =
            await FilterByEmail(email => email == "pjenyns0@state.gov");

        Assert.AreEqual<int>(1, actual.Length);
        Assert.AreEqual<(string, string)>(("Priscilla", "Jenyns"), actual[0]);
    }

    [TestMethod]
    public async Task FilterByEmailAddress_NoMatches_ReturnsEmpty()
    {
        Assert.IsFalse((await FilterByEmail(_ => false)).Any());
    }

    [TestMethod]
    public async Task FilterByEmailAddress_NullFilter_Throws()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => FilterByEmail(null!));
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollection_MatchesUniqueSortedListFromCsvRows()
    {
        string expected = string.Join(", ", await GetUniqueSortedStates());

        Assert.AreEqual<string>(expected, await GetAggregateStatesFor(await GetPeople()));
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollection_DuplicateStates_ListedOnce()
    {
        IPerson[] people =
        [
            new Person("A", "One", new Address("1 Main St", "Spokane", "WA", "99201"), "a@example.com"),
            new Person("B", "Two", new Address("2 Main St", "Helena", "MT", "59601"), "b@example.com"),
            new Person("C", "Three", new Address("3 Main St", "Cheney", "WA", "99004"), "c@example.com")
        ];

        Assert.AreEqual<string>("MT, WA", await GetAggregateStatesFor(people));
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollection_EmptyCollection_ReturnsEmptyString()
    {
        Assert.AreEqual<string>(string.Empty, await GetAggregateStatesFor([]));
    }

    [TestMethod]
    public async Task GetAggregateListOfStatesGivenPeopleCollection_NullPeople_Throws()
    {
        await Assert.ThrowsExactlyAsync<ArgumentNullException>(() => GetAggregateStatesFor(null));
    }
}
