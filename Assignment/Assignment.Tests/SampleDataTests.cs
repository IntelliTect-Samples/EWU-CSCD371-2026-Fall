using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    public void CsvRows_SkipsHeader_Returns50Rows()
    {
        SampleData data = new();

        Assert.AreEqual<int>(50, data.CsvRows.Count());
        Assert.IsFalse(data.CsvRows.Any(row => row.StartsWith("Id,", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void CsvRows_FirstRow_IsFirstPerson()
    {
        SampleData data = new();

        Assert.AreEqual<string>(
            "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,MT,70577",
            data.CsvRows.First());
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_MatchesHardcodedList()
    {
        SampleData data = new();
        string[] expected =
        [
            "AL", "AZ", "CA", "DC", "FL", "GA", "IN", "KS", "LA", "MD",
            "MN", "MO", "MT", "NC", "NE", "NH", "NV", "NY", "OR", "PA",
            "SC", "TN", "TX", "UT", "VA", "WA", "WV"
        ];

        string[] actual = data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual).All(pair => pair.First == pair.Second));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_IsSortedAndUnique()
    {
        SampleData data = new();
        string[] states = data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        Assert.IsTrue(states.Zip(states.Skip(1))
            .All(pair => string.CompareOrdinal(pair.First, pair.Second) < 0));
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_MatchesHardcodedString()
    {
        SampleData data = new();

        Assert.AreEqual<string>(
            "AL, AZ, CA, DC, FL, GA, IN, KS, LA, MD, MN, MO, MT, NC, NE, NH, NV, NY, OR, PA, SC, TN, TX, UT, VA, WA, WV",
            data.GetAggregateSortedListOfStatesUsingCsvRows());
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_MatchesUniqueSortedList()
    {
        SampleData data = new();
        string[] expected = data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        string[] actual = data.GetAggregateSortedListOfStatesUsingCsvRows().Split(", ");

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual).All(pair => pair.First == pair.Second));
    }

    [TestMethod]
    public void People_ReturnsOnePersonPerCsvRow()
    {
        SampleData data = new();

        Assert.AreEqual<int>(data.CsvRows.Count(), data.People.Count());
    }

    [TestMethod]
    public void People_AreSortedByStateThenCityThenZip()
    {
        SampleData data = new();
        IPerson[] people = data.People.ToArray();

        Assert.IsTrue(people.Zip(people.Skip(1)).All(pair =>
        {
            int state = string.CompareOrdinal(pair.First.Address.State, pair.Second.Address.State);
            int city = string.CompareOrdinal(pair.First.Address.City, pair.Second.Address.City);
            int zip = string.CompareOrdinal(pair.First.Address.Zip, pair.Second.Address.Zip);
            return state < 0 || (state == 0 && (city < 0 || (city == 0 && zip <= 0)));
        }));
    }

    [TestMethod]
    public void People_MatchCsvRowsIncludingAddress()
    {
        SampleData data = new();

        string[] expected = data.CsvRows
            .Select(row => string.Join(',', row.Split(',').Skip(1)))
            .Order(StringComparer.Ordinal)
            .ToArray();
        string[] actual = data.People
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
    public void People_FirstPerson_IsFromAlabama()
    {
        SampleData data = new();

        IPerson first = data.People.First();

        Assert.AreEqual<string>("AL", first.Address.State);
    }

    [TestMethod]
    public void Address_NullArgument_Throws()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address(null!, "City", "ST", "12345"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address("Street", null!, "ST", "12345"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address("Street", "City", null!, "12345"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address("Street", "City", "ST", null!));
    }

    [TestMethod]
    public void Person_NullArgument_Throws()
    {
        Address address = new("Street", "City", "ST", "12345");

        Assert.ThrowsExactly<ArgumentNullException>(() => new Person(null!, "Last", address, "a@b.com"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("First", null!, address, "a@b.com"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("First", "Last", null!, "a@b.com"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("First", "Last", address, null!));
    }
}
