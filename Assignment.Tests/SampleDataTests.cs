#pragma warning disable MSTEST0037

using Assignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace AssignmentTests;

[TestClass]
public class SampleDataTests
{
    private static readonly string[] ExpectedStatesForAggregate = new[] { "ID", "OR", "WA" };

    [TestMethod]
    public void SampleData_CreateNewInstance_CsvRowsLoadsData()
    {
        SampleData sampleData = new Assignment.SampleData();
        IEnumerable<string> rows = sampleData.CsvRows;
        Assert.IsNotNull(rows);
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_DefaultInstance_ReturnsListOfDistinctValues()
    {
        SampleData sampleData = new Assignment.SampleData();
        IEnumerable<string> states = sampleData.GetUniqueSortedListOfStatesGivenCsvRows();
        Assert.IsNotNull(states);
        CollectionAssert.AllItemsAreUnique(states.ToList());
        CollectionAssert.AreEqual(states.ToList(), states.Distinct().ToList());
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_DefaultInstance_ReturnsSortedLists()
    {
        SampleData sampleData = new Assignment.SampleData();
        IEnumerable<string> states = sampleData.GetUniqueSortedListOfStatesGivenCsvRows();
        Assert.IsTrue(IsNonDecreasingList(states));
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_DefaultInstance_ReturnsSingleString()
    {
        SampleData sampleData = new Assignment.SampleData();
        string aggregatedStates = sampleData.GetAggregateSortedListOfStatesUsingCsvRows();
        Console.WriteLine(aggregatedStates);
        Assert.IsNotNull(aggregatedStates);
        Assert.Contains(",", aggregatedStates);
    }

    //We are not testing to make sure that GetAggregateSortedListOfStatesUsingCsvRows is returning
    //Distinct states because that is being tested by GetUniqueSortedListOfStatesGivenCsvRows which 
    //GetAggregateSortedListOfStatesUsingCsvRows is using. Therefore we can assume that GetAggregateSortedListOfStatesUsingCsvRows
    //is distinct.

    [TestMethod]
    public void People_DefaultInstance_PopulatesPeopleProperty()
    {
        SampleData sampleData = new Assignment.SampleData();
        IEnumerable<IPerson> people = sampleData.People;
        Assert.IsNotNull(people);
        Assert.IsTrue(people.Any());
    }

    [TestMethod]
    public void People_DefaultInstance_ContainsAllPeopleData()
    {
        SampleData sampleData = new Assignment.SampleData();
        IEnumerable<string> csvRows = sampleData.CsvRows;
        List<IPerson> peopleList = sampleData.People.ToList(); 

        Assert.IsTrue(
            csvRows
                .Select(row => CreatePerson(row))
                .All(CreatedPerson => peopleList.Any(p =>
                    p.FirstName == CreatedPerson.FirstName &&
                    p.LastName == CreatedPerson.LastName &&
                    p.EmailAddress == CreatedPerson.EmailAddress &&
                    p.Address.City == CreatedPerson.Address.City &&
                    p.Address.State == CreatedPerson.Address.State &&
                    p.Address.Zip == CreatedPerson.Address.Zip
                ))
        );
    }

    [TestMethod]
    public void People_ShouldBeSortedByStateCityZip()
    {
        // Arrange
        SampleData sampleData = new Assignment.SampleData();
        List<IPerson> personList = sampleData.People.ToList();

        // Act

        // Assert:
        Assert.IsTrue(personList.Zip(personList.Skip(1), (prev, next) =>
            string.Compare(prev.Address.State, next.Address.State, StringComparison.Ordinal) <= 0 &&
            (prev.Address.State != next.Address.State ||
             string.Compare(prev.Address.City, next.Address.City, StringComparison.Ordinal) <= 0) &&
            (prev.Address.State != next.Address.State ||
             prev.Address.City != next.Address.City ||
             string.Compare(prev.Address.Zip, next.Address.Zip, StringComparison.Ordinal) <= 0))
        .All(result => result)
        );
    }

    [TestMethod]
    public void FilterByEmailAddress_PredicateAlwaysTrue_ReturnsAllPersons()
    {
        SampleData sampledata = new Assignment.SampleData();
        IEnumerable<IPerson> people = sampledata.People;

        var result = sampledata.FilterByEmailAddress(email => true).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(people.Count(), result.Count);
        Assert.IsTrue(people.All(p => result.Any(t => t.FirstName == p.FirstName && t.LastName == p.LastName)));
    }

    [TestMethod]
    public void FilterByEmailAddress_PredicateAlwaysFalse_ReturnsEmpty()
    {
        SampleData sampledata = new Assignment.SampleData();
        IEnumerable<IPerson> people = sampledata.People;

        var result = sampledata.FilterByEmailAddress(email => false).ToList();

        Assert.IsNotNull(result);
        Assert.AreEqual(0, result.Count);
    }

    [TestMethod]
    public void FilterByEmailAddress_NullPredicate_ThrowsNullArgumentException()
    {
        SampleData sampledata = new Assignment.SampleData();

        Assert.Throws<ArgumentNullException>(
            () => sampledata.FilterByEmailAddress(null!)
        );
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_ValidInput_ReturnsStates()
    {
        var people = new List<IPerson>
        {
            new Person("Alice", "One", new Address("Street 1", "CityA", "WA", "99205"), "alice@example.com"),
            new Person("Bob", "Two", new Address("Street 2", "CityB", "ID", "83854"), "bob@example.com"),
            new Person("Cam", "Three", new Address("Street 3", "CityC", "OR", "97001"), "cam@example.com"),
            new Person("Dan", "Four", new Address("Street 4", "CityD", "WA", "99001"), "dan@example.com"),
        };
        SampleData sampleData = new Assignment.SampleData();

        string result = sampleData.GetAggregateListOfStatesGivenPeopleCollection(people);

        Assert.IsNotNull(result);
        var states = result.Split(", ", StringSplitOptions.RemoveEmptyEntries);
        Assert.AreEqual(states.Distinct().Count(), states.Length);
        Assert.IsTrue(states.Zip(states.Skip(1), (prev, next) => string.Compare(prev, next, StringComparison.Ordinal) <= 0).All(b => b));
        CollectionAssert.AreEqual(ExpectedStatesForAggregate, states);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_EmptyCollection_ReturnsEmptyString()
    {
        IEnumerable<IPerson> people = Enumerable.Empty<IPerson>();
        SampleData sampledata = new Assignment.SampleData();

        string result = sampledata.GetAggregateListOfStatesGivenPeopleCollection(people);

        Assert.AreEqual(string.Empty, result);
    }
    
    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_NullPeople_ThrowsArgumentNullException()
    {
        SampleData sampledata = new Assignment.SampleData();

        Assert.Throws<ArgumentNullException>(
            () => sampledata.GetAggregateListOfStatesGivenPeopleCollection(null!)
        );
    }

    private static IPerson CreatePerson(string row)
    {
        string[] columns = row.Split(',');
        IAddress address = new Address(
            columns[4],
            columns[5],
            columns[6],
            columns[7]);
        IPerson person = new Person(
            columns[1],
            columns[2],
            address,
            columns[3]);
        return person;
    }

    private static bool IsNonDecreasingList(IEnumerable<string> input)
    {
        return input
            .Zip(input.Skip(1), (prev, next) => string.Compare(next, prev, StringComparison.Ordinal) >= 0)
            .All(result => result);
    }
}