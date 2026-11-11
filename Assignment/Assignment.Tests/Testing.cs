using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class Testing
{
    // Hardcoded rows: out of order, WA repeated.
    private sealed class FakeSampleData : SampleData
    {
        public override IEnumerable<string> CsvRows { get; } =
        [
            "1,Ann,Lee,ann@x.com,1 Main St,Spokane,WA,99201",
            "2,Bob,Ray,bob@y.org,2 Oak Ave,Boise,ID,83702",
            "3,Cy,Fox,cy@x.com,3 Elm Rd,Seattle,WA,98101",
            "4,Di,Poe,di@x.com,4 Pine Ln,Austin,TX,73301",
        ];
    }

    private static string[] RawFileLines() => File.ReadAllLines("People.csv");

    // ---------- 1. CsvRows ----------

    [TestMethod]
    public void CsvRows_SkipsHeader_AndKeepsEveryDataLine()
    {
        // No IDisposable to manage: File.ReadAllLines opens, reads and closes the file itself.
        string[] allLines = RawFileLines();
        string[] rows = new SampleData().CsvRows.ToArray();

        Assert.AreEqual<int>(allLines.Length - 1, rows.Count());
        Assert.IsFalse(rows.Contains(allLines[0]));
        Assert.IsTrue(rows.Zip(allLines.Skip(1), (actual, expected) => actual == expected)
                          .All(match => match));
    }

    // ---------- 2. Unique, sorted states ----------

    [TestMethod]
    public void GetUniqueSortedStates_HardcodedRows_ReturnsExpectedStates()
    {
        string[] expected = ["ID", "TX", "WA"];
        string[] actual = new FakeSampleData()
            .GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        Assert.AreEqual<int>(expected.Length, actual.Length);
        Assert.IsTrue(expected.Zip(actual, (e, a) => e == a).All(match => match));
    }

    [TestMethod]
    public void GetUniqueSortedStates_RealData_IsSortedAndUnique()
    {
        SampleData data = new();
        string[] states = data.GetUniqueSortedListOfStatesGivenCsvRows().ToArray();

        // Sorted: compare against an independently sorted copy (List<T>.Sort)
        List<string> sortedCopy = states.ToList();
        sortedCopy.Sort(StringComparer.Ordinal);
        Assert.IsTrue(states.Zip(sortedCopy, (a, b) => a == b).All(match => match));

        // Unique
        Assert.AreEqual<int>(states.Length, states.Distinct().Count());

        // Complete: every state in the file made it in
        IEnumerable<string> fileStates = data.CsvRows.Select(r => r.Split(',')[6].Trim());
        Assert.IsTrue(fileStates.All(s => states.Contains(s)));
    }
}
