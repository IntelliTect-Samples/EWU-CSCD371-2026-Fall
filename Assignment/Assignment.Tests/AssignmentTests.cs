namespace Assignment.Tests;

using System.Linq;
using Assignment;
using Microsoft.VisualStudio.TestTools.UnitTesting;


/// <summary>
/// This class contains unit tests for the SampleData class, which implements the ISampleData interface.
/// The tests verify the correctness of the methods and properties defined in the SampleData class, ensuring
/// that they behave as expected when processing CSV data and returning collections of people and states.
/// </summary>
[TestClass]
public class AssignmentTests
{

    // Use field type encapsualtion
    public SampleData SampleData { get; } = new();


    /// <summary>
    /// Tests that the CsvRows property returns the expected raw data rows from the CSV file, skipping the header row.
    /// It checks that the returned rows are not empty, do not contain the header row,
    /// </summary>
    [TestMethod]
    public void CsvRows_SkipsHeaderAndReturnsRawDataRows()
    {
        var rows = SampleData.CsvRows.ToList();

        Assert.IsTrue(rows.Count > 0);
        Assert.DoesNotContain(static row => row.StartsWith("Id,", System.StringComparison.Ordinal), rows);
        Assert.AreEqual("1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,MT,70577", rows[0]);
    }


    /// <summary>
    /// Tests that the GetUniqueSortedListOfStatesGivenCsvRows method returns a distinct and alphabetically sorted list of states from the CSV rows.
    /// It compares the actual output with the expected output derived from the CsvRows property.
    /// </summary>
    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_ReturnsDistinctAlphabetizedStates()
    {
        var actual = SampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();
        var expected = SampleData.CsvRows
            .Select(static row => row.Split(',')[6])
            .Distinct()
            .OrderBy(static state => state)
            .ToArray();

        CollectionAssert.AreEqual(expected, actual);
    }


    /// <summary>
    /// Tests that the GetAggregateSortedListOfStatesUsingCsvRows method returns a single string containing a comma-separated list of unique states, sorted alphabetically.
    /// It compares the actual output with the expected output derived from the GetUniqueSortedListOf
    /// </summary>
    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_ReturnsCommaSeparatedUniqueStates()
    {
        var actual = SampleData.GetAggregateSortedListOfStatesUsingCsvRows();
        var expected = string.Join(",", SampleData.GetUniqueSortedListOfStatesGivenCsvRows());

        Assert.AreEqual(expected, actual);
    }


    /// <summary>
    /// Tests that the People property returns a collection of IPerson objects sorted by state, city, and zip code.
    /// It checks that the returned collection is not empty, and that each person has a populated
    /// </summary>
    [TestMethod]
    public void People_AreSortedByStateCityAndZip_AndHavePopulatedAddressDetails()
    {
        var actual = SampleData.People
            .Select(static person => (person.FirstName, person.LastName, person.EmailAddress, person.Address.StreetAddress, person.Address.City, person.Address.State, person.Address.Zip))
            .ToList();

        var expected = SampleData.CsvRows
            .Select(static row => row.Split(','))
            .Select(static columns => (
                FirstName: columns[1],
                LastName: columns[2],
                EmailAddress: columns[3],
                StreetAddress: columns[4],
                City: columns[5],
                State: columns[6],
                Zip: columns[7]))
            .OrderBy(static person => person.State)
            .ThenBy(static person => person.City)
            .ThenBy(static person => person.Zip)
            .Select(static person => (person.FirstName, person.LastName, person.EmailAddress, person.StreetAddress, person.City, person.State, person.Zip))
            .ToList();

        CollectionAssert.AreEqual(expected.Select(static person => person.ToString()).ToList(), actual.Select(static person => person.ToString()).ToList());
        Assert.IsTrue(actual.All(static person => !string.IsNullOrWhiteSpace(person.State)));
    }


    /// <summary>
    /// Tests that the FilterByEmailAddress method returns a collection of people whose email addresses match the provided predicate.
    /// It checks that the returned collection contains the expected people based on the filter criteria.
    /// </summary>
    [TestMethod]
    public void FilterByEmailAddress_ReturnsPeopleMatchingThePredicate()
    {
        var actual = SampleData.FilterByEmailAddress(static email => email.EndsWith("@state.gov", System.StringComparison.Ordinal)).ToList();

        Assert.AreEqual(1, actual.Count);
        Assert.AreEqual(("Priscilla", "Jenyns"), actual[0]);
    }


    /// <summary>
    /// Tests that the GetAggregateListOfStatesGivenPeopleCollection method returns a comma-separated list of distinct states from the provided collection of people.
    /// It checks that the returned string matches the expected output derived from the provided people collection.
    /// </summary>
    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_UsesDistinctSortedStates()
    {
        var people = SampleData.People.ToList();

        var actual = SampleData.GetAggregateListOfStatesGivenPeopleCollection(people);
        var expected = people
            .Select(static person => person.Address.State)
            .Distinct()
            .OrderBy(static state => state)
            .Aggregate(static (current, next) => $"{current},{next}");

        Assert.AreEqual(expected, actual);
    }
}
