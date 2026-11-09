using System;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    #region CsvRows Tests
    
    // CsvRows requirements:
    
    // Implementation shall:
    // 1. return each line from a source .csv file as a single string.
    // 2. exclude the first row if the source .csv file.
    
    [TestMethod]
    public void CsvRows_WhenEnumerated_ReturnsAllRowsExceptHeader()
    {
        // arrange
        string[] expectedDataRows =
        [
            "FirstDataRow",
            "SecondDataRow"
        ];
        
        using TestCsvFile sourceFile = 
            new(["Ignored Header Row", .. expectedDataRows]);
        
        SampleData sampleData = new(sourceFile.FilePath);
        
        // act
        string[] actual = [.. sampleData.CsvRows];
        
        // assert
        Assert.IsTrue(expectedDataRows.SequenceEqual(actual));
    }

    [TestMethod]
    public void CsvRows_WhenEnumeratedWithJustHeader_ReturnsEmptyCollection()
    {
        using TestCsvFile sourceFile = new("Ignored Header Row");
        
        SampleData sampleData = new(sourceFile.FilePath);
        
        string[] actual = [..sampleData.CsvRows];
        
        Assert.IsEmpty(actual);
    }

    #endregion

    #region GetUniqueSortedListOfStatesGivenCsvRows Tests
    
    // Note CSV structure of: Id,FirstName,LastName,Email,StreetAddress,City,State,Zip
    // where State is column 7 (index 6)

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_WhenStatesContainDuplicates_ReturnsUnique()
    {
        string[] csvDataRows = 
        [
            CreateCsvDataRowWithState("CA"),
            CreateCsvDataRowWithState("CA"),
            CreateCsvDataRowWithState("OR"),
            CreateCsvDataRowWithState("WA"),
        ];

        string[] expectedResult = 
        [
            "CA",
            "OR",
            "WA",
        ];
        
        using TestCsvFile sourceFile = 
            new(["Ignored Header Row", .. csvDataRows]);
        
        SampleData sampleData = new(sourceFile.FilePath);
        
        string[] actual = 
            [..sampleData.GetUniqueSortedListOfStatesGivenCsvRows()];
        
        Assert.IsTrue(expectedResult.SequenceEqual(actual));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_WhenStatesAreUnordered_ReturnsAlphabeticallyOrdered()
    {
        string[] csvDataRows =
        [
            CreateCsvDataRowWithState("WA"),
            CreateCsvDataRowWithState("CA"),
            CreateCsvDataRowWithState("OR"),
        ];

        string[] expectedResult =
        [
            "CA",
            "OR",
            "WA",
        ];
        
        using TestCsvFile sourceFile = 
            new(["Ignored Header Row", .. csvDataRows]);
        
        SampleData sampleData = new(sourceFile.FilePath);
        
        string[] actual = 
            [..sampleData.GetUniqueSortedListOfStatesGivenCsvRows()];
        
        Assert.IsTrue(expectedResult.SequenceEqual(actual));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_WhenStatesReturned_AreAlphabeticallyOrdered()
    {
        SampleData sampleData = new();
        
        string[] actual = 
            [..sampleData.GetUniqueSortedListOfStatesGivenCsvRows()];

        string[] expected =
            [.. actual.OrderBy(state => state)];
        
        Assert.IsTrue(expected.SequenceEqual(actual));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_WhenNoDataRowsExists_ReturnsEmptyCollection()
    {
        using TestCsvFile sourceFile = new("Ignored Header Row");
        
        SampleData sampleData = new(sourceFile.FilePath);
        
        string[] actual = 
            [..sampleData.GetUniqueSortedListOfStatesGivenCsvRows()];
        
        Assert.IsEmpty(actual);
    }

    #endregion

    #region GetAggregateSortedListOfStatesUsingCsvRows Tests

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_ReturnsAggregateString()
    {
        string[] csvDataRows =
        [
            CreateCsvDataRowWithState("CA"),
            CreateCsvDataRowWithState("CA"),
            CreateCsvDataRowWithState("OR"),
            CreateCsvDataRowWithState("WA")
        ];

        const string expected = "CA,OR,WA";
        
        using TestCsvFile sourceFile =
            new(["Ignored Header Row", .. csvDataRows]);

        SampleData sampleData = new(sourceFile.FilePath);
        
        string actual = sampleData
            .GetAggregateSortedListOfStatesUsingCsvRows();
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_WhenNoDataRowsExists_ReturnsEmptyCollection()
    {
        using TestCsvFile sourceFile = new("Ignored Header Row");
        
        SampleData sampleData = new(sourceFile.FilePath);
        
        string actual = sampleData
            .GetAggregateSortedListOfStatesUsingCsvRows();
        
        Assert.IsEmpty(actual);
    }

    #endregion

    #region People Tests

    [TestMethod]
    public void People_WhenCsvRowExists_ReturnsMappedPerson()
    {
        Person expected = CreateTestPerson();
        
        using TestCsvFile sourceFile = 
            new("Ignored Header Row",
                CreateCsvDataRow(expected));
        
        SampleData sampleData = new(sourceFile.FilePath);

        Person actual = sampleData.People.Cast<Person>().Single();
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void People_WhenStatesAreUnordered_ReturnsSortedByState()
    {
        Person[] expected = 
        [
            CreateTestPerson(state: "A"),
            CreateTestPerson(state: "Z")
        ];
        
        // file intentionally swaps the expected ordering
        using TestCsvFile sourceFile = 
            new("Ignored Header Row",
                CreateCsvDataRow(expected[1]),
                CreateCsvDataRow(expected[0])
            );
        
        SampleData sampleData = new(sourceFile.FilePath);

        Person[] actual =
            [.. sampleData.People.Cast<Person>()];
        
        Assert.IsTrue(expected.SequenceEqual(actual));
    }
    
    [TestMethod]
    public void People_WhenCitiesAreUnordered_ReturnsSortedByCity()
    {
        Person[] expected = 
        [
            CreateTestPerson(city: "A"),
            CreateTestPerson(city: "Z")
        ];
        
        // file intentionally swaps the expected ordering
        using TestCsvFile sourceFile = 
            new("Ignored Header Row",
                CreateCsvDataRow(expected[1]),
                CreateCsvDataRow(expected[0])
            );
        
        SampleData sampleData = new(sourceFile.FilePath);

        Person[] actual =
            [.. sampleData.People.Cast<Person>()];
        
        Assert.IsTrue(expected.SequenceEqual(actual));
    }
    
    [TestMethod]
    public void People_WhenZipsAreUnordered_ReturnsSortedByZip()
    {
        Person[] expected = 
        [
            CreateTestPerson(zip: "1"),
            CreateTestPerson(zip: "2")
        ];
        
        // file intentionally swaps the expected ordering
        using TestCsvFile sourceFile = 
            new("Ignored Header Row",
                CreateCsvDataRow(expected[1]),
                CreateCsvDataRow(expected[0])
            );
        
        SampleData sampleData = new(sourceFile.FilePath);

        Person[] actual =
            [.. sampleData.People.Cast<Person>()];
        
        Assert.IsTrue(expected.SequenceEqual(actual));
    }
    
    [TestMethod]
    public void People_WhenEnumerated_ReturnsSortedByStateCityAndZip()
    {
        Person[] expected =
        [
            CreateTestPerson(
                state: "A",
                city: "A",
                zip: "1"),
            
            CreateTestPerson(
                state: "A",
                city: "A",
                zip: "2"),
            
            CreateTestPerson(
                state: "A",
                city: "Z",
                zip: "1"),
            
            CreateTestPerson(
                state: "Z",
                city: "A",
                zip: "1"),
        ];
        
        using TestCsvFile sourceFile = 
            new("Ignored Header Row",
                CreateCsvDataRow(expected[3]),
                CreateCsvDataRow(expected[2]),
                CreateCsvDataRow(expected[1]),
                CreateCsvDataRow(expected[0])
            );
        
        SampleData sampleData = new(sourceFile.FilePath);
        
        Person[] actual =
            [.. sampleData.People.Cast<Person>()];
        
        Assert.IsTrue(expected.SequenceEqual(actual));
    }

    [TestMethod]
    public void People_WhenCsvContainsOnlyHeader_ReturnsEmptyCollection()
    {
        using TestCsvFile sourceFile = 
            new("Ignored Header Row");
        
        SampleData sampleData = new(sourceFile.FilePath);
        
        Person[] actual =
            [.. sampleData.People.Cast<Person>()];
        
        Assert.IsEmpty(actual);
    }

    #endregion

    #region FilterByEmailAddress Tests

    [TestMethod]
    public void FilterByEmailAddress_WhenEmailMatches_ReturnName()
    {
        Person expectedPerson = CreateTestPerson(
            firstName: "Test",
            lastName: "Person",
            emailAddress: "find me");

        using TestCsvFile sourceFile =
            new("Ignored Header Row",
                CreateCsvDataRow(expectedPerson));
        
        SampleData sampleData = new(sourceFile.FilePath);

        (string FirstName, string LastName)[] actual =
        [..sampleData.FilterByEmailAddress(email => email == expectedPerson.EmailAddress)];

        (string FirstName, string LastName)[] expected =
        [
            (expectedPerson.FirstName,
            expectedPerson.LastName)
        ];
        
        // sequenceEqual used with one entry expected
        Assert.IsTrue(expected.SequenceEqual(actual));
    }

    [TestMethod]
    public void FilterByEmailAddress_WhenNoEmailMatches_ReturnEmptyCollection()
    {
        Person personEntry = CreateTestPerson();

        using TestCsvFile sourceFile =
            new("Ignored Header Row",
                CreateCsvDataRow(personEntry));
        
        SampleData sampleData = new(sourceFile.FilePath);
        
        (string FirstName, string LastName)[] actual =
            [..sampleData.FilterByEmailAddress(email => email == "not an email")];
        
        Assert.IsEmpty(actual);
    }

    [TestMethod]
    public void FilterByEmailAddress_WhenMultipleMatches_ReturnsMatchingEntryNames()
    {
        const string matchEmail = "find me";
        
        Person matchingEntryOne = CreateTestPerson(
            firstName: "First",
            lastName: "Match",
            emailAddress: matchEmail);
        
        Person matchingEntryTwo = CreateTestPerson(
            firstName: "Second",
            lastName: "Match",
            emailAddress: matchEmail);

        Person notAMatchingEntry = CreateTestPerson();

        using TestCsvFile sourceFile =
            new("Ignored Header Row",
                CreateCsvDataRow(matchingEntryOne),
                CreateCsvDataRow(matchingEntryTwo),
                CreateCsvDataRow(notAMatchingEntry));
        
        SampleData sampleData = new(sourceFile.FilePath);
        
        (string FirstName, string LastName)[] actual =
            [..sampleData.FilterByEmailAddress(email => email == matchEmail)];

        // note: sorting done means that order can change from actual
        (string FirstName, string LastName)[] expected =
        [
            (matchingEntryOne.FirstName, matchingEntryOne.LastName),
            (matchingEntryTwo.FirstName, matchingEntryTwo.LastName)
        ];
        
        // same entries found, but order could be different
        Assert.HasCount(expected.Length, actual);
        Assert.IsTrue(expected.All(actual.Contains));
    }

    #endregion

    #region GetAggregateListOfStatesGivenPeopleCollection Tests

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_ReturnsUniqueAggregateString()
    {
        Person[] peopleEntries = 
        [
            CreateTestPerson(state: "A"),
            CreateTestPerson(state: "A"),
            CreateTestPerson(state: "B"),
            CreateTestPerson(state: "C")
        ];

        const string expected = "A,B,C";

        SampleData sampleData = new();
        
        string actual = 
            sampleData.GetAggregateListOfStatesGivenPeopleCollection(peopleEntries);
        
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_WhenPeopleCollectionEmpty_ReturnsEmptyCollection()
    {
        Person[] peopleEntries = [];
        
        SampleData sampleData = new();
        
        string actual = sampleData
            .GetAggregateListOfStatesGivenPeopleCollection(peopleEntries);
        
        Assert.IsEmpty(actual);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_WhenSingleStateExists_ReturnsSingle()
    {
        Person[] peopleEntries =
        [
            CreateTestPerson(state: "A")
        ];
        
        SampleData sampleData = new();
        
        string actual = sampleData
            .GetAggregateListOfStatesGivenPeopleCollection(peopleEntries);
        
        Assert.AreEqual("A", actual);
    }

    #endregion
    
    private static string CreateCsvDataRowWithState(string state) =>
         $"x,x,x,x,x,x,{state},x";

    private static string CreateCsvDataRow(Person person)
    {
        return string.Join(",", 
            [
                "x",
                person.FirstName,
                person.LastName,
                person.EmailAddress,
                person.Address.StreetAddress,
                person.Address.City,
                person.Address.State,
                person.Address.Zip
            ]
        );
    }

    private static Person CreateTestPerson(
        string firstName = "defaultFirstName",
        string lastName = "defaultLastName",
        string emailAddress = "defaultEmail",
        string streetAddress = "defaultAddress",
        string city = "defaultCity",
        string state = "defaultState",
        string zip = "defaultZip")
    {
        return new Person(
            firstName,
            lastName,
            emailAddress,
            
            new Address(
                streetAddress,
                city,
                state,
                zip)
        );
    }

    private sealed class TestCsvFile : IDisposable
    {
        public string FilePath { get; }
        
        public TestCsvFile(params string[] rows)
        {
            FilePath = Path.GetTempFileName();
            
            File.WriteAllLines(FilePath, rows);
        }

        public void Dispose()
        {
            File.Delete(FilePath);
        }
    }
}