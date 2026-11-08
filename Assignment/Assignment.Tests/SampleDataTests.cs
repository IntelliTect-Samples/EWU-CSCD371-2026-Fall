using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    private const string CsvFile = "People.csv";
    
    #region CsvRows Tests
    
    // CsvRows requirements:
    // given: a source csv file where each row is represented as a single string.
    // Implementation shall:
    // 1. shall return each line from a source .csv file as a single string.
    // 2. shall exclude the first row if the source .csv file.
    
    [TestMethod]
    public void CsvRows_WhenEnumerated_FirstReturnedResultIsSecondRowOfSourceFile()
    {
        // If CsvRows returns each source row as a single string,
        // and excludes the first source row,
        
        // then the first returned string must
        // equal the second row of the source file.
        
        // arrange
        SampleData sampleData = new();
        
        // second line of source file
        string expected = 
            File.ReadLines(CsvFile)
            .ElementAt(1);
        
        // act + get first string from CsvRows
        string actual = 
            sampleData.CsvRows
            .First();
        
        // assert
        Assert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void CsvRows_WhenEnumerated_ReturnsOneLessRowThanSourceFile()
    {
        // If CsvRows returns each source row as a single string,
        // and excludes the first source row,
        
        // then the returned collection count must
        // equal the source file count minus one.
        
        // arrange
        SampleData sampleData = new();

        // count of source file minus one
        int expected =
            File.ReadLines(CsvFile)
                .Count() - 1;

        // act + count from CsvRows
        int actual = 
            sampleData.CsvRows
                .Count();
        
        // assert
        Assert.AreEqual(expected, actual);
    }
    
    #endregion
}