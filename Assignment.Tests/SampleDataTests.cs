using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System; // <-- ADDED: Needed for StringComparison
using Assignment; 

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    private const string TestFileName = "People.csv";
    private SampleData _sampleData = null!;

    [TestInitialize] //Init the suite before tests run
    public void Setup()
    {
        // Create dummy CSV object
        var lines = new[]
        {
            "FirstName,LastName,Email,Street,City,State,Zip",
            "John,Doe,john@test.com,123 Main,Seattle,WA,98101",
            "Jane,Smith,jane@test.com,456 Oak,Portland,OR,97201",
            "Bob,Jones,bob@example.com,789 Pine,Spokane,WA,99201"
        };

        File.WriteAllLines(TestFileName, lines);
        _sampleData = new SampleData(TestFileName);
    }

    [TestCleanup] //Clean up your dirty mess when you're 
    public void Cleanup()
    {
        if (File.Exists(TestFileName)) File.Delete(TestFileName);
    }

    [TestMethod]
    public void CsvRows_SkipsHeader_And_ReturnsCorrectCount()
    {
        var rows = _sampleData.CsvRows.ToList();
        
        // FIX 1: Swapped arguments to (expectedCount, collection)
        Assert.HasCount(3, rows);
        
        // FIX 2 (Line 44): Added StringComparison.Ordinal to resolve CA1310
        Assert.IsFalse(rows.Any(r => r.StartsWith("FirstName", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStates_ReturnsSortedUniqueList()
    {
        var states = _sampleData.GetUniqueSortedListOfStatesGivenCsvRows().ToList();

        // Hardcoded check
        Assert.AreEqual("OR", states[0]);
        Assert.AreEqual("WA", states[1]);
        
        // FIX 3: Swapped arguments to (expectedCount, collection)
        Assert.HasCount(2, states);

        // LINQ Verification of Sort (using Zip to compare current vs next)
        // FIX 4 (Line 60): Added StringComparison.Ordinal to resolve CA1309/CA1310
        var isSorted = states.Zip(states.Skip(1), (a, b) => string.Compare(a, b, StringComparison.Ordinal) < 0).All(x => x);
        Assert.IsTrue(isSorted, "List was not sorted alphabetically.");
    }

    [TestMethod]
    public void GetAggregateSortedListOfStates_ReturnsCommaSeparatedString()
    {
        var result = _sampleData.GetAggregateSortedListOfStatesUsingCsvRows();
        Assert.AreEqual("OR, WA", result);
    }

    [TestMethod]
    public void People_ReturnsObjects_SortedByStateCityZip()
    {
        var people = _sampleData.People.ToList();

        // FIX 5: Swapped arguments to (expectedCount, collection)
        Assert.HasCount(3, people);
        
        // Verify Sort Order (OR comes before WA)
        Assert.AreEqual("OR", people[0].Address.State);
        Assert.AreEqual("WA", people[1].Address.State);

        // Verify Data Mapping
        Assert.AreEqual("Jane", people[0].FirstName);
    }

    [TestMethod]
    public void FilterByEmailAddress_ReturnsMatches()
    {
        var result = _sampleData.FilterByEmailAddress(email => email.Contains("@test.com")).ToList();

        // FIX 6: Swapped arguments to (expectedCount, collection)
        Assert.HasCount(2, result);
        
        // Verify using Contains/Any
        Assert.IsTrue(result.Any(x => x.FirstName == "John"));
        Assert.IsTrue(result.Any(x => x.FirstName == "Jane"));
        Assert.IsFalse(result.Any(x => x.FirstName == "Bob"));
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_UsesAggregate()
    {
        // Step 6 Implementation verification
        var people = _sampleData.People;
        var result = _sampleData.GetAggregateListOfStatesGivenPeopleCollection(people);

        Assert.AreEqual("OR, WA", result);
    }

}