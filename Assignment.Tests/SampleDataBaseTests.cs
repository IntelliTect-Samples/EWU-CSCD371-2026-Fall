using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Assignment.Tests;

public static class AssertExtensions
{
    public static Assert That = null!;

    public static void HasCount<T>(this Assert assert, int expected, IEnumerable<T> collection)
    {
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.AreEqual(expected, collection.Count());
    }

    public static void Contains(this Assert assert, (string FirstName, string LastName) expected, IEnumerable<(string FirstName, string LastName)> collection)
    {
        Microsoft.VisualStudio.TestTools.UnitTesting.Assert.IsTrue(collection.Contains(expected));
    }
}

[TestClass]
public abstract class BaseSampleDataTests<TService> where TService : class
{
    protected TService DataService { get; set; } = default!;

    protected abstract TService CreateService();
    protected abstract IEnumerable<string> GetCsvRows();
    protected abstract IEnumerable<string> GetUniqueSortedListOfStates();
    protected abstract IEnumerable<IPerson> GetPeople();
    protected abstract IEnumerable<(string FirstName, string LastName)> FilterByEmail(Predicate<string> filter);
    protected abstract string GetAggregateListOfStates(IEnumerable<IPerson> people);
    protected abstract string GetAggregateSortedListOfStates();

    [TestInitialize]
    public void Initialize()
    {
        DataService = CreateService();
    }

    protected static bool IsListSorted(List<string> list)
    {
        for (int i = 1; i < list.Count; i++)
        {
            if (StringComparer.OrdinalIgnoreCase.Compare(list[i - 1], list[i]) > 0) return false;
        }
        return true;
    }

    protected static bool IsSetDistinct(IEnumerable<string> list)
    {
        return list.Count() == list.Distinct().Count();
    }


    [TestMethod]
    public void TestIsListSorted_SortedList_True()
    {
        Assert.IsTrue(IsSetDistinct(new string[] { "a", "ab", "abc" }));
    }

    [TestMethod]
    public void TestIsListSorted_NotSortedList_False()
    {
        Assert.IsFalse(IsSetDistinct(new string[] { "b", "a", "abc", "b" }));
    }

    [TestMethod]
    public void TestDistinct_IsDistinct_True()
    {
        Assert.IsTrue(IsSetDistinct(new string[] { "a", "b", "ab" }));
    }

    [TestMethod]
    public void TestDistinct_IsNotDistinct_False()
    {
        Assert.IsFalse(IsSetDistinct(new string[] { "b", "c", "a", "b" }));
    }


    // 1.
    [TestMethod]
    public void CsvRows_loadsPeopleCvs_NotNull()
    {
        // Act
        IEnumerable<string> csvOut = GetCsvRows().ToList();
        // Assert
        Assert.IsNotNull(csvOut);
    }

    [TestMethod]
    public void CsvRows_loadsPeopleCvs_IgnoresFirstRow()
    {
        // Act
        IEnumerable<string> csvOut = GetCsvRows().ToList();
        // Assert
        AssertExtensions.That.HasCount(50, csvOut);
        Assert.AreNotEqual("Id,FirstName,LastName,Email,StreetAddress,City,State,Zip", csvOut.First());
    }

    // 2.
    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_LoadsCVS_IsSorted()
    {
        // Act
        IEnumerable<string> csvOut = GetUniqueSortedListOfStates();
        // Assert
        Assert.IsTrue(IsListSorted(csvOut.ToList()));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_LoadsCVS_IsDistinct()
    {
        // Act
        IEnumerable<string> csvOut = GetUniqueSortedListOfStates();
        // Assert
        Assert.IsTrue(IsSetDistinct(csvOut));
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_HardCodedCompare_IsDistinct()
    {
        // Act
        IEnumerable<string> csvOut = GetUniqueSortedListOfStates();
        // Assert
        Assert.AreEqual("AL AZ CA DC FL GA IN KS LA MD MN MO MT NC NE NH NV NY OR PA SC TN TX UT VA WA WV", string.Join(" ", csvOut));
        AssertExtensions.That.HasCount(27, csvOut);
    }

    [TestMethod]
    public void GetUniqueSortedListOfStatesGivenCsvRows_LoadsCVS_IsSortedLinqTest()
    {
        // Act
        IEnumerable<string> csvOut = GetUniqueSortedListOfStates();
        // Assert
        Assert.IsTrue(csvOut.OrderBy(n => n, StringComparer.OrdinalIgnoreCase).SequenceEqual(csvOut));
    }

    // 3.
    [TestMethod]
    public void GetAggregateSortedListOfStatesUsingCsvRows_MatchesUniqueList()
    {
        // Act
        string aggregateOutput = GetAggregateSortedListOfStates();
        string expectedOutput = string.Join(",", GetUniqueSortedListOfStates());

        // Assert
        Assert.AreEqual(expectedOutput, aggregateOutput);
    }

    // 4.
    [TestMethod]
    public void People_MaintainsLength_EqualLength()
    {
        // Act
        IEnumerable<IPerson> people = GetPeople();
        IEnumerable<string> peopleAsString = GetCsvRows();
        // Assert
        AssertExtensions.That.HasCount(peopleAsString.Count(), people);

        var isSorted = people.Zip(people.Skip(1), (prev, next) =>
            StringComparer.Ordinal.Compare(prev.Address.State, next.Address.State) <= 0 &&
            (StringComparer.Ordinal.Compare(prev.Address.State, next.Address.State) < 0 ||
             StringComparer.Ordinal.Compare(prev.Address.City, next.Address.City) <= 0) &&
            (StringComparer.Ordinal.Compare(prev.Address.State, next.Address.State) < 0 ||
             StringComparer.Ordinal.Compare(prev.Address.City, next.Address.City) < 0 ||
             StringComparer.Ordinal.Compare(prev.Address.Zip, next.Address.Zip) <= 0)
        ).All(b => b);

        Assert.IsTrue(isSorted, "People collection is not correctly sorted by State, City, then Zip.");
    }

    // 5.
    [TestMethod]
    public void FilterByEmailAddress_SingleEmail_Success()
    {
        // Arrange
        static bool emailPredicate(string email) { return "mjeannotp@google.ca".Equals(email, StringComparison.Ordinal); }

        // Act
        IEnumerable<(string FirstName, string LastName)> fullNames = FilterByEmail(emailPredicate);

        // Assert
        AssertExtensions.That.Contains(("Molly", "Jeannot"), fullNames);
        AssertExtensions.That.HasCount(1, fullNames);
    }

    // 6.
    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_TwoPeopleFromSameState_IsDistinct()
    {
        // Arrange
        List<IPerson> peopleList = [];
        Person person = new("First", "Last", new Address("12E Somewhere", "Spocan", "WA", "99208"), "fake@email.com");
        Person personTwo = new("Person", "People", new Address("13E Somewhere", "Spocan", "WA", "99501"), "real@email.com");
        peopleList.Add(person);
        peopleList.Add(personTwo);

        // Act
        string stateOutput = GetAggregateListOfStates(peopleList);

        // Assert
        Assert.AreEqual("WA", stateOutput);
    }

    [TestMethod]
    public void GetAggregateListOfStatesGivenPeopleCollection_PeopleCSV_DistinctCount()
    {
        // Act
        string stateOutput = GetAggregateListOfStates(GetPeople());

        // Assert
        Assert.AreEqual(string.Join(",", GetUniqueSortedListOfStates()), stateOutput);
    }
}