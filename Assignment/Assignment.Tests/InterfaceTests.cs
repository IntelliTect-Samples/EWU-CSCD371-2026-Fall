namespace Assignment.Tests;

using System.Linq;
using Assignment;
using Assignment.Interfaces;
using Microsoft.VisualStudio.TestTools.UnitTesting;

[TestClass]
public class InterfaceTests
{
    [TestMethod]
    public void IAsyncSampleData_IsDefinedAsAnInterface()
    {
        Assert.IsTrue(typeof(IAsyncSampleData).IsInterface);
    }

    [TestMethod]
    public void IPerson_ContainsExpectedProperties()
    {
        var properties = typeof(IPerson).GetProperties();

        Assert.IsTrue(typeof(IPerson).IsInterface);
        CollectionAssert.AreEquivalent(new[] { nameof(IPerson.FirstName), nameof(IPerson.LastName), nameof(IPerson.EmailAddress), nameof(IPerson.Address) }, properties.Select(property => property.Name).ToArray());
    }

    [TestMethod]
    public void IAddress_ContainsExpectedProperties()
    {
        var properties = typeof(IAddress).GetProperties();

        Assert.IsTrue(typeof(IAddress).IsInterface);
        CollectionAssert.AreEquivalent(new[] { nameof(IAddress.StreetAddress), nameof(IAddress.City), nameof(IAddress.State), nameof(IAddress.Zip) }, properties.Select(property => property.Name).ToArray());
    }

    [TestMethod]
    public void ISampleData_ContainsTheExpectedMembers()
    {
        Assert.IsTrue(typeof(ISampleData).IsInterface);
        Assert.IsNotNull(typeof(ISampleData).GetProperty(nameof(ISampleData.CsvRows)));
        Assert.IsNotNull(typeof(ISampleData).GetMethod(nameof(ISampleData.GetUniqueSortedListOfStatesGivenCsvRows)));
        Assert.IsNotNull(typeof(ISampleData).GetMethod(nameof(ISampleData.GetAggregateSortedListOfStatesUsingCsvRows)));
        Assert.IsNotNull(typeof(ISampleData).GetProperty(nameof(ISampleData.People)));
        Assert.IsNotNull(typeof(ISampleData).GetMethod(nameof(ISampleData.FilterByEmailAddress)));
        Assert.IsNotNull(typeof(ISampleData).GetMethod(nameof(ISampleData.GetAggregateListOfStatesGivenPeopleCollection)));
    }
}