using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
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
    public void GetUniqueSortedListOfStatesGivenCsvRows_LoadsCVS_IsSortedLinqTest()
    {
        //Assign
        SampleData sampleData = new SampleData();
        IEnumerable<string> csvOut = sampleData.GetUniqueSortedListOfStatesGivenCsvRows();

        //Assert
        Assert.IsTrue(csvOut.OrderBy(n => n).SequenceEqual(csvOut));
    }
    [TestMethod]
    public void StringToPerson_NullInput_Throws()
    {
        //Assign
        SampleData sampleData = new SampleData();
        //Assert
        Assert.Throws<ArgumentNullException>(() => sampleData.StringToPerson(null!));
    }
    [TestMethod]
    public void StringToPerson_ImproperLength_Throws()
    {
        //Assign
        SampleData sampleData = new SampleData();
        string personMissingOneElement = "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,70577";
        string personMissingTwoElements = "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena";

        //Assert
        Assert.Throws<InvalidDataException>( () => sampleData.StringToPerson(personMissingOneElement));
        Assert.Throws<InvalidDataException>(() => sampleData.StringToPerson(personMissingTwoElements));
    }
    [TestMethod]
    public void StringToPerson_ProperLength_ParsesCorrectly()
    {
        //Assign
        SampleData sampleData = new SampleData();
        string personMissingOneElement = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";
        //Assert
        IPerson person = sampleData.StringToPerson(personMissingOneElement);
        Assert.AreEqual<string>("FirstName", person.FirstName);
        Assert.AreEqual<string>("LastName", person.LastName);
        Assert.AreEqual<string>("Email", person.EmailAddress);
        Assert.AreEqual<string>("StreetAddress", person.Address.StreetAddress);
        Assert.AreEqual<string>("City", person.Address.City);
        Assert.AreEqual<string>("State", person.Address.State);
        Assert.AreEqual<string>("Zip", person.Address.Zip);
    }
    [TestMethod]
    public void People_CheckOrdering_IsOrdered()
    {
        //Assign
        SampleData sampleData = new SampleData();
        IEnumerable<IPerson> people = sampleData.People;
        //Act
        IEnumerable<IPerson> clonePeople = people.OrderBy(person => person.Address.State)
                .ThenBy(person => person.Address.City)
                .ThenBy(person => person.Address.Zip);
        //Assert
        Assert.IsTrue(clonePeople.SequenceEqual(people));
    }
    [TestMethod]
    public void People_MaintainsLength_EqualLength()
    {
        //Assign
        SampleData sampleData = new SampleData();
        IEnumerable<IPerson> people = sampleData.People;
        IEnumerable<string> peopleAsString = sampleData.CsvRows;
        //Act
        bool hasDuplicates = people.Count() != people.Distinct().Count();
        //Assert
        Assert.AreEqual<int>(peopleAsString.Count(), people.Count());
        Assert.IsFalse(hasDuplicates);
    }
    [TestMethod]
    public void People_NoDuplicates_Success()
    {
        // NOTE: This test may fail if duplicates are added to people.csv
        // This test was created to ensure 'People' is being populated with potentally useful information
        // without having to implement the entire functionality of 'People'.

        //Assign
        SampleData sampleData = new SampleData();
        IEnumerable<IPerson> people = sampleData.People;
        //Act
        bool hasDuplicates = people.Count() != people.Distinct().Count();
        //Assert
        Assert.IsFalse(hasDuplicates);
    }
}
