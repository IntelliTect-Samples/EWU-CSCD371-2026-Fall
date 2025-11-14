using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Assignment.Tests;

[TestClass]
public class SampleDataTests
{
    private static bool IsListSorted(List<string> list)
    {
        for(int i = 1; i < list.Count(); i++)
        {
            if (StringComparer.OrdinalIgnoreCase.Compare(list[i - 1], list[i]) > 0) return false;
        }
        return true;
    }
    private static bool IsSetDistinct(IEnumerable<string> list)
    {
        HashSet<string> visited = new();
        foreach (string s in list)
        {
            if (visited.Contains(s))
                return false;

            visited.Add(s);
        }
        return true;
    }
    #region Test methods

    [TestMethod]
    public void TestIsListSorted_SortedList_True()
    {
        Assert.IsTrue(IsSetDistinct(new List<string> { "a", "ab", "abc" }));
    }

    [TestMethod]
    public void TestIsListSorted_NotSortedList_False()
    {
        Assert.IsFalse(IsSetDistinct(new List<string> { "b", "a", "abc", "b" }));
    }

    [TestMethod]
    public void TestDistinct_IsDistinct_True()
    {
        Assert.IsTrue(IsSetDistinct(new List<string> { "a", "b", "ab" }));
    }

    [TestMethod]
    public void TestDistinct_IsNotDistinct_False()
    {
        Assert.IsFalse(IsSetDistinct(new List<string> { "b", "c", "a" , "b"}));
    }
    #endregion

    [TestMethod]
    public void CsvRows_loadsPeopleCvs_NotNull()
    {
        //Assign
        SampleData sampleData = new SampleData();
        IEnumerable<string> csvOut = sampleData.CsvRows.ToList();
        //Assert
        Assert.IsNotNull(csvOut);
    }

    [TestMethod]
    public void CsvRows_loadsPeopleCvs_IgnoresFirstRow()
    {
        //Assign
        SampleData sampleData = new SampleData();
        IEnumerable<string> csvOut = sampleData.CsvRows.ToList();
        //Assert
        Assert.HasCount(50, csvOut);//CSV is of size 51, we should be getting 50.
        Assert.AreNotEqual("Id,FirstName,LastName,Email,StreetAddress,City,State,Zip", csvOut.First());
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_LoadsCVS_IsSorted()
    {
        //Assign
        SampleData sampleData = new SampleData();
        IEnumerable<string> csvOut = sampleData.GetUniqueSortedListOfStatesGivenCsvRows();

        //Assert
        Assert.IsTrue(IsListSorted(csvOut.ToList()));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_LoadsCVS_IsDistinct()
    {
        //Assign
        SampleData sampleData = new SampleData();
        IEnumerable<string> csvOut = sampleData.GetUniqueSortedListOfStatesGivenCsvRows();

        //Assert
        Assert.IsTrue(IsSetDistinct(csvOut));
    }
    //Include a test that uses LINQ to verify the data is sorted correctly (do not use a hardcoded list)
    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_LoadsCVS_HardcodedIsSorted()
    {
        //Assign
        SampleData sampleData = new SampleData();
        IEnumerable<string> csvOut = sampleData.GetUniqueSortedListOfStatesGivenCsvRows();

        //Assert
        Assert.IsTrue(csvOut.OrderBy(n => n).SequenceEqual(csvOut));
    }
}
