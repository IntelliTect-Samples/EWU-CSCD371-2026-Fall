using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks; // Needed for async Task
using System;
using Assignment;

namespace Assignment.Tests;

[TestClass]
public class SampleDataAsyncTests
{
    private const string TestFileName = "PeopleAsync.csv";
    private SampleDataAsync _sampleData = null!;

    [TestInitialize]
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
        _sampleData = new SampleDataAsync(TestFileName);
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(TestFileName)) File.Delete(TestFileName);
    }

    [TestMethod]
    public async Task CsvRows_SkipsHeader_And_ReturnsCorrectCount()
    {
        // 1. We must await the consumption of the IAsyncEnumerable
        var rows = await ToListAsync(_sampleData.CsvRows);

        // FIX 1: Swapped arguments to (expectedCount, collection)
        Assert.HasCount(3, rows);

        // FIX 2: StringComparison
        Assert.IsFalse(rows.Any(r => r.StartsWith("FirstName", StringComparison.Ordinal)));
    }

    [TestMethod]
    public async Task GetUniqueSortedListOfStates_ReturnsSortedUniqueList()
    {
        // 1. Consume the async stream
        var states = await ToListAsync(_sampleData.GetUniqueSortedListOfStatesGivenCsvRows());

        // Hardcoded check
        Assert.AreEqual("OR", states[0]);
        Assert.AreEqual("WA", states[1]);

        // FIX 3: Swapped arguments
        Assert.HasCount(2, states);

        // LINQ Verification of Sort
        // FIX 4: StringComparison
        var isSorted = states.Zip(states.Skip(1), (a, b) => string.Compare(a, b, StringComparison.Ordinal) < 0).All(x => x);
        Assert.IsTrue(isSorted, "List was not sorted alphabetically.");
    }

    [TestMethod]
    public void GetAggregateSortedListOfStates_ReturnsCommaSeparatedString()
    {
        // NOTE: Your implementation of this method in SampleDataAsync is Synchronous (returns string).
        // It blocks internally using .ToEnumerable(). Therefore, we do not await here.
        var result = _sampleData.GetAggregateSortedListOfStatesUsingCsvRows();
        
        Assert.AreEqual("OR, WA", result);
    }

    [TestMethod]
    public async Task People_ReturnsObjects_SortedByStateCityZip()
    {
        // 1. Consume the async stream
        var people = await ToListAsync(_sampleData.People);

        // FIX 5: Swapped arguments
        Assert.HasCount(3, people);

        // Verify Sort Order (OR comes before WA)
        Assert.AreEqual("OR", people[0].Address.State);
        Assert.AreEqual("WA", people[1].Address.State);

        // Verify Data Mapping
        Assert.AreEqual("Jane", people[0].FirstName);
    }

    [TestMethod]
    public async Task FilterByEmailAddress_ReturnsMatches()
    {
        // 1. Pass the predicate
        var asyncResult = _sampleData.FilterByEmailAddress(email => email.Contains("@test.com"));
        
        // 2. Consume stream
        var result = await ToListAsync(asyncResult);

        // FIX 6: Swapped arguments
        Assert.HasCount(2, result);

        // Verify using Contains/Any
        Assert.IsTrue(result.Any(x => x.FirstName == "John"));
        Assert.IsTrue(result.Any(x => x.FirstName == "Jane"));
        Assert.IsFalse(result.Any(x => x.FirstName == "Bob"));
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_UsesAggregate()
    {
        // NOTE: The method signature in SampleDataAsync accepts IAsyncEnumerable 
        // but returns a 'string' synchronously.
        
        var peopleStream = _sampleData.People;
        
        // No await here, because the method itself is not async, even though the parameter is.
        var result = _sampleData.GetAggregateListOfStatesGivenPeopleCollection(peopleStream);

        Assert.AreEqual("OR, WA", result);
    }

    // ---------------------------------------------------------
    // HELPER: Converts IAsyncEnumerable<T> to List<T> for testing
    // ---------------------------------------------------------
    private static async Task<List<T>> ToListAsync<T>(IAsyncEnumerable<T> items)
    {
        var results = new List<T>();
        await foreach (var item in items)
        {
            results.Add(item);
        }
        return results;
    }
}

