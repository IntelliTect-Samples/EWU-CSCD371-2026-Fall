using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;
using Assignment;


[TestClass]
public class SampleDataTests
{
    private readonly SampleData sampleData = new();

    [TestMethod]
    public void CsvRows_SkipsHeader()
    {
        string[] rows = sampleData.CsvRows.ToArray();
        Assert.IsTrue(rows.Length > 0);
        Assert.IsFalse(rows[0].StartsWith("Id,", StringComparison.Ordinal));
        Assert.IsTrue(rows[0].StartsWith("1,Priscilla,Jenyns,", StringComparison.Ordinal));
        Assert.AreEqual(50, rows.Length);
    }

    [TestMethod]
    public void UniqueStates_MatchExpectedHardcodedAddresses()
    {
        string[] expected = ["AL", "AZ", "CA", "DC", "FL", "GA", "IN", "KS", "LA", "MD", "MN", "MO", "MT", "NC", "NE", "NH", "NV", "NY", "OR", "PA", "SC", "TN", "TX", "UT", "VA", "WA", "WV"];
        string[] actual = sampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();
        Assert.IsTrue(expected.SequenceEqual(actual));
    }

    [TestMethod]
    public void UniqueStates_AreSortedAndHaveNoDuplicates()
    {
        string[] states = sampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();
        Assert.IsTrue(states.SequenceEqual(states.OrderBy(s => s, StringComparer.Ordinal)));
        Assert.AreEqual(states.Length, states.Distinct(StringComparer.Ordinal).Count());
    }

    [TestMethod]
    public void AggregateStatesFromRows_IsSortedAndCommaSeparated()
    {
        string expected = string.Join(",", sampleData.GetUniqueSortedListOfStatesGivenCsvRows());
        Assert.AreEqual(expected, sampleData.GetAggregateSortedListOfStatesUsingCsvRows());
    }

    [TestMethod]
    public void People_AreMappedAndSortedByStateCityZip()
    {
        IPerson[] people = sampleData.People.ToArray();
        Assert.AreEqual(sampleData.CsvRows.Count(), people.Length);
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
        var names = sampleData.FilterByEmailAddress(email => email.EndsWith("@stanford.edu", StringComparison.Ordinal)).ToArray();
        Assert.AreEqual(2, names.Length);
        Assert.IsTrue(names.Contains(("Sancho", "Mahony")));
        Assert.IsTrue(names.Contains(("Fayette", "Dougherty")));
    }

    [TestMethod]
    public void AggregateStatesFromPeople_MatchesCsvStates()
    {
        string expected = sampleData.GetAggregateSortedListOfStatesUsingCsvRows();
        Assert.AreEqual(expected, sampleData.GetAggregateListOfStatesGivenPeopleCollection(sampleData.People));
    }

    [TestMethod]
    public void AggregateStatesFromPeople_RemovesDuplicates()
    {
        IPerson[] people = sampleData.People.Where(p => p.Address.State is "CA" or "WA").ToArray();
        Assert.AreEqual("CA,WA", sampleData.GetAggregateListOfStatesGivenPeopleCollection(people));
    }

    [TestMethod]
    public void EmptyCollection_ProducesEmptyAggregate()
    {
        Assert.AreEqual(string.Empty, sampleData.GetAggregateListOfStatesGivenPeopleCollection([]));
    }

    [TestMethod]
    public void NullArguments_Throw()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => sampleData.FilterByEmailAddress(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => sampleData.GetAggregateListOfStatesGivenPeopleCollection(null!));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address(null!, "City", "WA", "12345"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("A", "B", "a@example.com", null!));
    }
}
