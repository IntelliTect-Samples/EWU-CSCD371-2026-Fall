using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    [TestMethod]
    public void CsvRowsReturnsAllRowsWithoutHeader()
    {
        // Arrange
        var sampleData = new SampleData();

        var csvPath = Path.Combine(
            AppContext.BaseDirectory,
            "People.csv");

        var expectedRows = File.ReadAllLines(csvPath)
            .Skip(1)
            .ToArray();

        // Act
        var actualRows = sampleData.CsvRows.ToArray();

        // Assert
        Assert.AreEqual<int>(
            expectedRows.Length,
            actualRows.Length);

        Assert.AreEqual<int>(
            0,
            expectedRows.Zip(actualRows)
                .Count(row => row.First != row.Second));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRowsReturnsExpectedStates()
    {
        // Arrange
        var sampleData = new SampleData();

        string[] expectedStates =
        [
            "AL", "AZ", "CA", "DC", "FL", "GA", "IN",
            "KS", "LA", "MD", "MN", "MO", "MT", "NC",
            "NE", "NH", "NV", "NY", "OR", "PA", "SC",
            "TN", "TX", "UT", "VA", "WA", "WV"
        ];

        // Act
        var actualStates = sampleData
            .GetUniqueSortedListOfStatesGivenCsvRows()
            .ToArray();

        // Assert
        Assert.AreEqual<int>(expectedStates.Length, actualStates.Length);

        Assert.AreEqual<int>(
            0,
            expectedStates.Zip(actualStates)
                .Count(pair => pair.First != pair.Second));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRowsReturnsSortedStates()
    {
        // Arrange
        var sampleData = new SampleData();

        // Act
        var states = sampleData
            .GetUniqueSortedListOfStatesGivenCsvRows()
            .ToArray();

        // Assert
        Assert.IsTrue(
            states.SequenceEqual(states.OrderBy(state => state)));

        Assert.AreEqual<int>(
            states.Length,
            states.Distinct().Count());
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRowsReturnsExpectedString()
    {
        // Arrange
        var sampleData = new SampleData();

        const string expected =
            "AL, AZ, CA, DC, FL, GA, IN, KS, LA, MD, MN, MO, MT, " +
            "NC, NE, NH, NV, NY, OR, PA, SC, TN, TX, UT, VA, WA, WV";

        // Act
        string actual = sampleData.GetAggregateSortedListOfStatesUsingCsvRows();

        // Assert
        Assert.AreEqual<string>(expected, actual);
    }

    [TestMethod]
    public void PeopleReturnsAllCsvRowsAsPopulatedPersonObjects()
    {
        // Arrange
        var sampleData = new SampleData();

        var expectedRows = sampleData.CsvRows
            .Select(row => row.Split(','))
            .OrderBy(columns => columns[6])
            .ThenBy(columns => columns[5])
            .ThenBy(columns => columns[7])
            .ToArray();

        // Act
        var people = sampleData.People.ToArray();

        // Assert
        Assert.AreEqual<int>(expectedRows.Length, people.Length);

        int mismatches = expectedRows.Zip(people)
            .Count(pair =>
            {
                string[] columns = pair.First;
                IPerson person = pair.Second;

                return person.FirstName != columns[1]
                    || person.LastName != columns[2]
                    || person.EmailAddress != columns[3]
                    || person.Address.StreetAddress != columns[4]
                    || person.Address.City != columns[5]
                    || person.Address.State != columns[6]
                    || person.Address.Zip != columns[7];
            });

        Assert.AreEqual<int>(0, mismatches);
    }

    [TestMethod]
    public void PeopleReturnsPeopleSortedByStateCityAndZip()
    {
        // Arrange
        var sampleData = new SampleData();

        // Act
        var people = sampleData.People.ToArray();

        var sortedPeople = people
            .OrderBy(person => person.Address.State)
            .ThenBy(person => person.Address.City)
            .ThenBy(person => person.Address.Zip)
            .ToArray();

        // Assert
        Assert.IsTrue(people.SequenceEqual(sortedPeople));
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollectionReturnsExpectedStates()
    {
        // Arrange
        var sampleData = new SampleData();

        string expected = sampleData.GetAggregateSortedListOfStatesUsingCsvRows();

        // Act
        string actual = sampleData
            .GetAggregateListOfStatesGivenPeopleCollection(sampleData.People);

        // Assert
        Assert.AreEqual<string>(expected, actual);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollectionRemovesDuplicates()
    {
        // Arrange
        var people = new IPerson[]
        {
            new Person("John", "Smith",
                new Address("123 Main St", "Seattle", "WA", "98101"),
                "john@example.com"),

            new Person("Jane", "Doe",
                new Address("456 Main St", "Portland", "OR", "97201"),
                "jane@example.com"),

            new Person("Bob", "Jones",
                new Address("789 Main St", "Tacoma", "WA", "98402"),
                "bob@example.com")
        };

        var sampleData = new SampleData();

        // Act
        string actual = sampleData
            .GetAggregateListOfStatesGivenPeopleCollection(people);

        // Assert
        Assert.AreEqual<string>("OR, WA", actual);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollectionEmptyCollectionReturnsEmptyString()
    {
        // Arrange
        var sampleData = new SampleData();

        // Act
        string actual = sampleData
            .GetAggregateListOfStatesGivenPeopleCollection(
                Array.Empty<IPerson>());

        // Assert
        Assert.AreEqual<string>(string.Empty, actual);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollectionNullCollectionThrowsArgumentNullException()
    {
        // Arrange
        var sampleData = new SampleData();

        // Act
        var exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => sampleData.GetAggregateListOfStatesGivenPeopleCollection(null!));

        // Assert
        Assert.AreEqual<string>("people", exception.ParamName);
    }

    [TestMethod]
    public void FilterByEmailAddressMatchingEmailReturnsPersonName()
    {
        // Arrange
        var sampleData = new SampleData();

        Predicate<string> filter = email =>
            email == "pjenyns0@state.gov";

        // Act
        var actual = sampleData
            .FilterByEmailAddress(filter)
            .ToArray();

        // Assert
        Assert.AreEqual<int>(1, actual.Length);
        Assert.AreEqual<string>("Priscilla", actual[0].FirstName);
        Assert.AreEqual<string>("Jenyns", actual[0].LastName);
    }

    [TestMethod]
    public void FilterByEmailAddress_NoMatchingEmails_ReturnsEmptyCollection()
    {
        // Arrange
        var sampleData = new SampleData();

        Predicate<string> filter = email =>
            email.EndsWith(
                "@example.invalid",
                StringComparison.OrdinalIgnoreCase);

        // Act
        var actual = sampleData
            .FilterByEmailAddress(filter)
            .ToArray();

        // Assert
        Assert.AreEqual<int>(0, actual.Length);
    }

    [TestMethod]
    public void FilterByEmailAddress_NullFilter_ThrowsArgumentNullException()
    {
        // Arrange
        var sampleData = new SampleData();

        // Act
        var exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => sampleData.FilterByEmailAddress(null!));

        // Assert
        Assert.AreEqual<string>("filter", exception.ParamName);
}

}