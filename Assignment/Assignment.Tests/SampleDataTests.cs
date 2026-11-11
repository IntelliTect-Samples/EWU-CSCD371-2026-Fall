using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    private readonly SampleData testData = new();

    [TestMethod]
    public void CsvRows_SkipsHeader()
    {
        string[] rows = testData.CsvRows.ToArray();
        Assert.IsTrue(rows.Length > 0);
        Assert.IsFalse(rows[0].StartsWith("Id,", StringComparison.Ordinal));
        Assert.IsTrue(rows[0].StartsWith("1,Priscilla,Jenyns,", StringComparison.Ordinal));
        Assert.AreEqual(50, rows.Length);
    }

    [TestMethod]
    public void UniqueStates_MatchExpectedHardcodedAddresses() //Whitespace is added in the implementation, but we removed it here to match the addresses only.
    {
        string[] expected = ["AL", "AZ", "CA", "DC", "FL", "GA", "IN", "KS", "LA", "MD", "MN", "MO", "MT", "NC", "NE", "NH", "NV", "NY", "OR", "PA", "SC", "TN", "TX", "UT", "VA", "WA", "WV"];
        string[] actual = testData.GetUniqueSortedListOfStatesGivenCsvRows().ToArray().Select(s => s.Trim()).ToArray(); 
        Assert.IsTrue(expected.SequenceEqual(actual));
    }

    [TestMethod]
    public void UniqueStates_AreSortedAndHaveNoDuplicates()
    {
        string[] states = testData.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();
        Assert.IsTrue(states.SequenceEqual(states.OrderBy(s => s, StringComparer.Ordinal)));
        Assert.AreEqual(states.Length, states.Distinct(StringComparer.Ordinal).Count());
    }

    [TestMethod]
    public void AggregateStatesFromRows_IsSortedAndCommaSeparated()
    {
        string expected = string.Join(", ", testData.GetUniqueSortedListOfStatesGivenCsvRows());
        Assert.AreEqual(expected, testData.GetAggregateSortedListOfStatesUsingCsvRows());
    }

    [TestMethod]
    public void People_AreMappedAndSortedByStateCityZip()
    {
        IPerson[] people = testData.People.ToArray();
        Assert.AreEqual(testData.CsvRows.Count(), people.Length);
        Assert.IsTrue(people.SequenceEqual(people.OrderBy(p => p.Address.State, StringComparer.Ordinal)
            .ThenBy(p => p.Address.City, StringComparer.Ordinal)
            .ThenBy(p => p.Address.Zip, StringComparer.Ordinal)));
        IPerson priscilla = people.Single(p => p.EmailAddress == "pjenyns0@state.gov");
        Assert.AreEqual("Priscilla", priscilla.FirstName);
        Assert.AreEqual("Jenyns", priscilla.LastName);
        Assert.AreEqual("7884 Corry Way", priscilla.Address.StreetAddress);
        Assert.AreEqual("Helena", priscilla.Address.City);
        Assert.AreEqual("MT", priscilla.Address.State);
        Assert.AreEqual("70577", priscilla.Address.Zip);
    }

    [TestMethod]
    public void FilterByEmailAddress_ReturnsMatchingNames()
    {
        var names = testData.FilterByEmailAddress(email => email.EndsWith("@stanford.edu", StringComparison.Ordinal)).ToArray();
        Assert.AreEqual(2, names.Length);
        Assert.IsTrue(names.Contains(("Sancho", "Mahony")));
        Assert.IsTrue(names.Contains(("Fayette", "Dougherty")));
    }

    [TestMethod]
    public void AggregateStatesFromPeople_MatchesCsvStates()
    {
        string expected = testData.GetAggregateSortedListOfStatesUsingCsvRows();
        Assert.AreEqual(expected, testData.GetAggregateListOfStatesGivenPeopleCollection(testData.People));
    }

    [TestMethod]
    public void AggregateStatesFromPeople_RemovesDuplicates()
    {
        IPerson[] people = testData.People.Where(p => p.Address.State is "CA" or "WA").ToArray();
        Assert.AreEqual("CA, WA", testData.GetAggregateListOfStatesGivenPeopleCollection(people));
    }

    [TestMethod]
    public void EmptyCollection_ProducesEmptyAggregate()
    {
        Assert.AreEqual(string.Empty, testData.GetAggregateListOfStatesGivenPeopleCollection([]));
    }

    [TestMethod]
    public void NullArguments_Throw()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => testData.FilterByEmailAddress(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => testData.GetAggregateListOfStatesGivenPeopleCollection(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address(null!, "City", "WA", "12345"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("A", "B", "a@example.com", null!));
    }
}
