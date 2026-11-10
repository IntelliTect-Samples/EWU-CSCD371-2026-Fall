using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;



namespace Assignment.Tests;

[TestClass]
public abstract class SampleDataTestsBase
{
    protected abstract Task<IReadOnlyList<string>> CsvRows();
    protected abstract Task<IReadOnlyList<string>> GetUniqueSortedListOfStatesGivenCsvRows();
    protected abstract Task<string> GetAggregateSortedListOfStatesUsingCsvRows();
    protected abstract Task<IReadOnlyList<IPerson>> People();
    protected abstract Task<IReadOnlyList<(string FirstName, string LastName)>> FilterByEmailAddress(Predicate<string> filter);


    [TestMethod]
    public async Task TestCsvRowsSkipsHeader()
    {
        var rows = await CsvRows();
        Assert.DoesNotContain("FirstName", rows[0]);
    }

    [TestMethod]
    public async Task TestGetUniqueSortedListOfStatesGivenCsvRowsHardcodedList()
    {
        List<string> states = [
            "CA",
        "WA",
        "CA",
        "TX",
        "WA"];

        List<string> expected =
        [
            "CA",
        "TX",
        "WA"
        ];

        Assert.AreSequenceEqual(expected, [.. states.Distinct().OrderBy(s => s)]);
    }

    [TestMethod]
    public async Task TestPeopleCountMatchesCsvRows()
    {
        var rows = await CsvRows();
        var people = await People();


        Assert.HasCount(rows.Count,
            people);
    }

    [TestMethod]
    public async Task TestPeopleHaveAddressesAsync()
    {
        var people = await People();

        Assert.IsTrue(
            people.All(
                p => p.Address != null));
    }

    [TestMethod]
    public async Task TestFilterByEmailAddress()
    {
        var result =
            await FilterByEmailAddress(
                email => email.EndsWith(".edu", StringComparison.OrdinalIgnoreCase));

        Assert.IsNotEmpty(result);
    }

    [TestMethod]
    public async Task TestGetUniqueSortedListOfStatesGivenCsvRowsWithLinq()
    {
        var rows = await CsvRows();
        var expected = rows.Select(row => row.Split(',')[6]).Distinct().OrderBy(State => State).ToList();
        var actual = await GetUniqueSortedListOfStatesGivenCsvRows();
        Assert.AreSequenceEqual(expected, actual);
    }

    [TestMethod]
    public async Task TestGetAggregateSortedListOfStatesUsingCsvRows()
    {
        var states = await GetUniqueSortedListOfStatesGivenCsvRows();
        var expected = states.Distinct().OrderBy(State => State).Aggregate((a, b) => a + ", " + b);
        var actual = await GetAggregateSortedListOfStatesUsingCsvRows();
        Assert.AreEqual(expected, actual);
    }
}
