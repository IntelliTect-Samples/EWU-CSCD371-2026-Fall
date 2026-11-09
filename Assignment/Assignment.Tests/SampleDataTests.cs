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
    
    private static string CreateCsvDataRowWithState(string state) =>
         $"x,x,x,x,x,x,{state},x";

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