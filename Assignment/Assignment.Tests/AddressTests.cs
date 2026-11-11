using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Assignment.Tests;

[TestClass]
public class AddressTests
{

    [TestMethod]
    [DataRow(null, "a", "b", "c")]
    [DataRow("a", null, "b", "c")]
    [DataRow("a", "b", null, "c")]
    [DataRow("a", "b", "c", null)]
    [DataRow("a", "b", null, null)]
    public void Constructor_NullArgument_ThrowsException(string streetAddress
        , string city
        , string state
        , string zip)
    {
        Address a;
        Assert.Throws<ArgumentNullException>(() => a = new(streetAddress, city, state, zip));
    }

    [TestMethod]
    [DataRow("a", "b", "c", "d")]
    [DataRow("", "b", "c", "d")]
    [DataRow("1234, street road", "TownVille", "Stateington", "12345")]
    public void Constructor_ValidArgument_ProperValuesAssigned(string streetAddress
        , string city
        , string state
        , string zip)
    {
        Address a = new(streetAddress, city, state, zip);
        Assert.AreEqual(streetAddress, a.StreetAddress);
        Assert.AreEqual(city, a.City);
        Assert.AreEqual(state, a.State);
        Assert.AreEqual(zip, a.Zip);
    }

}