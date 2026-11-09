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
    
    // given: a source csv file where each row is represented as a single string.
    
    // Implementation shall:
    // 1. shall return each line from a source .csv file as a single string.
    // 2. shall exclude the first row if the source .csv file.
    
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