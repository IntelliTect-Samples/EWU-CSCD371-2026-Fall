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
        for(int i = 1; i < list.Count; i++)
        {
            if (StringComparer.OrdinalIgnoreCase.Compare(list[i - 1], list[i]) > 0) return false;
        }
        return true;
    }
    private static bool IsSetDistinct(IEnumerable<string> list)
    {
        HashSet<string> visited = [];
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
        Assert.IsTrue(IsSetDistinct(["a", "ab", "abc"]));
    }

    [TestMethod]
    public void TestIsListSorted_NotSortedList_False()
    {
        Assert.IsFalse(IsSetDistinct(["b", "a", "abc", "b"]));
    }

    [TestMethod]
    public void TestDistinct_IsDistinct_True()
    {
        Assert.IsTrue(IsSetDistinct(["a", "b", "ab"]));
    }

    [TestMethod]
    public void TestDistinct_IsNotDistinct_False()
    {
        Assert.IsFalse(IsSetDistinct(["b", "c", "a", "b"]));
    }
    #endregion

    [TestMethod]
    public void CsvRows_loadsPeopleCvs_NotNull()
    {
        //Assign
        SampleData sampleData = new();
        IEnumerable<string> csvOut = sampleData.CsvRows.ToList();
        //Assert
        Assert.IsNotNull(csvOut);
    }

    [TestMethod]
    public void CsvRows_loadsPeopleCvs_IgnoresFirstRow()
    {
        //Assign
        SampleData sampleData = new();
        IEnumerable<string> csvOut = sampleData.CsvRows.ToList();
        //Assert
        Assert.HasCount(50, csvOut);//CSV is of size 51, we should be getting 50.
        Assert.AreNotEqual("Id,FirstName,LastName,Email,StreetAddress,City,State,Zip", csvOut.First());
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_LoadsCVS_IsSorted()
    {
        //Assign
        SampleData sampleData = new();
        IEnumerable<string> csvOut = sampleData.GetUniqueSortedListOfStatesGivenCsvRows();

        //Assert
        Assert.IsTrue(IsListSorted(csvOut.ToList()));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_LoadsCVS_IsDistinct()
    {
        //Assign
        SampleData sampleData = new();
        IEnumerable<string> csvOut = sampleData.GetUniqueSortedListOfStatesGivenCsvRows();

        //Assert
        Assert.IsTrue(IsSetDistinct(csvOut));
    }
    //Include a test that uses LINQ to verify the data is sorted correctly (do not use a hardcoded list)
    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_LoadsCVS_IsSortedLinqTest()
    {
        //Assign
        SampleData sampleData = new();
        IEnumerable<string> csvOut = sampleData.GetUniqueSortedListOfStatesGivenCsvRows();

        //Assert
        Assert.IsTrue(csvOut.OrderBy(n => n).SequenceEqual(csvOut));
    }
    [TestMethod]
    public void StringToPerson_NullInput_Throws()
    {
        //Assign
        SampleData sampleData = new();
        //Assert
        Assert.Throws<ArgumentNullException>(() => SampleData.StringToPerson(null!));
    }
    [TestMethod]
    public void StringToPerson_ImproperLength_Throws()
    {
        //Assign
        SampleData sampleData = new();
        string personMissingOneElement = "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena,70577";
        string personMissingTwoElements = "1,Priscilla,Jenyns,pjenyns0@state.gov,7884 Corry Way,Helena";

        //Assert
        Assert.Throws<InvalidDataException>( () => SampleData.StringToPerson(personMissingOneElement));
        Assert.Throws<InvalidDataException>(() => SampleData.StringToPerson(personMissingTwoElements));
    }
    [TestMethod]
    public void StringToPerson_ProperLength_ParsesCorrectly()
    {
        //Assign
        string personMissingOneElement = "Id,FirstName,LastName,Email,StreetAddress,City,State,Zip";
        //Assert
        IPerson person = SampleData.StringToPerson(personMissingOneElement);
        Assert.AreEqual<string>("FirstName", person.FirstName);
        Assert.AreEqual<string>("LastName", person.LastName);
        Assert.AreEqual<string>("Email", person.EmailAddress);
        Assert.AreEqual<string>("StreetAddress", person.Address.StreetAddress);
        Assert.AreEqual<string>("City", person.Address.City);
        Assert.AreEqual<string>("State", person.Address.State);
        Assert.AreEqual<string>("Zip", person.Address.Zip);
    }
    [TestMethod]
    public void People_MaintainsLength_EqualLength()
    {
        //Assign
        SampleData sampleData = new();
        IEnumerable<IPerson> people = sampleData.People;
        IEnumerable<string> peopleAsString = sampleData.CsvRows;
        //Act
        bool hasDuplicates = people.Count() != people.Distinct().Count();
        //Assert
        Assert.HasCount(peopleAsString.Count(), people);
        Assert.IsFalse(hasDuplicates);
    }
    [TestMethod]
    public void People_NoDuplicates_Success()
    {
        // NOTE: This test may fail if duplicates are added to people.csv
        // This test was created to ensure 'People' is being populated with potentally useful information
        // without having to implement the entire functionality of 'People'.

        //Assign
        SampleData sampleData = new();
        IEnumerable<IPerson> people = sampleData.People;
        //Act
        bool hasDuplicates = people.Count() != people.Distinct().Count();
        //Assert
        Assert.IsFalse(hasDuplicates);
    }
    [TestMethod]
    public void FilterByEmailAddress_SingleEmail_Success()
    {
        //Assign
        SampleData sampleData = new();
        static bool emailPredicate(string email) { return "mjeannotp@google.ca".Equals(email); }
        (string,string) expectedName = ("Molly", "Jeannot");

        //Act
        IEnumerable<(string FirstName, string LastName)> fullNames = sampleData.FilterByEmailAddress(emailPredicate);
        
        //Assert
        Assert.Contains(expectedName, fullNames);
        Assert.HasCount(1, fullNames);
    }
    [TestMethod]
    public void FilterByEmailAddress_MultipleEmails_Success()
    {
        //Assign
        SampleData sampleData = new();
        Predicate<string> emailPredicate = (string email) => {  return "mjeannotp@google.ca".Equals(email) 
                                                                    || "mrawsthorneq@slate.com".Equals(email); };
        (string, string)[] expectedName = [("Molly", "Jeannot"), ("Maria", "Rawsthorne")];
        //Act
        IEnumerable<(string FirstName, string LastName)> fullNames = sampleData.FilterByEmailAddress(emailPredicate);

        //Assert
        Assert.IsTrue(expectedName.SequenceEqual(fullNames));
        Assert.HasCount(2, fullNames);
    }
    [TestMethod]
    public void FilterByEmailAddress_FakeEmail_ReturnsNoUsers()
    {
        //Assign
        SampleData sampleData = new();
        Predicate<string> emailPredicate = (string email) => {
            return "offical_rnicrosoft@bing.com".Equals(email);
        };
        //Act
        IEnumerable<(string FirstName, string LastName)> fullNames = sampleData.FilterByEmailAddress(emailPredicate);

        //Assert
        Assert.HasCount(0, fullNames);
    }
    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_TwoPeopleFromSameState_IsDistinct()
    {
        //Assign
        SampleData sampleData = new();
        List<IPerson> peopleList = [];
        Person person = new("First", "Last", new Address("12E Somewhere", "Spocan", "WA", "99208"), "fake@email.com");
        Person personTwo = new("Person", "People", new Address("13E Somewhere", "Spocan", "WA", "99501"), "real@email.com");
        peopleList.Add(person);
        peopleList.Add(personTwo);
        //Act
        string stateOutput = sampleData.GetAggregateListOfStatesGivenPeopleCollection(peopleList);

        //Assert
        Assert.AreEqual<string>("WA", stateOutput);
    }
    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_TwoPeopleFromDifferingState_BothIncluded()
    {
        //Assign
        SampleData sampleData = new();
        List<IPerson> peopleList = [];
        Person person = new("First", "Last", new Address("12E Somewhere", "Spocan", "WA", "99208"), "fake@email.com");
        Person personTwo = new("Person", "People", new Address("13E Somewhere", "Somewhhere", "CA", "99501"), "real@email.com");
        peopleList.Add(person);
        peopleList.Add(personTwo);
        //Act
        string stateOutput = sampleData.GetAggregateListOfStatesGivenPeopleCollection(peopleList);

        //Assert
        Assert.AreEqual<string>("WA,CA", stateOutput);
    }
    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_PeopleCSV_DistinctCount()
    {
        //Assign
        SampleData sampleData = new();
        //Act
        string stateOutput = sampleData.GetAggregateListOfStatesGivenPeopleCollection(sampleData.People);

        //Assert
        Assert.AreEqual<string>(sampleData.GetAggregateSortedListOfStatesUsingCsvRows(), stateOutput);
    }
}
