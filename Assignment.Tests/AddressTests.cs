using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class AddressTests
{
    [TestMethod]
    public void ConstructorValidArgumentsSetsProperties()
    {
        var address = new Address(
            "123 Main Street",
            "Seattle",
            "WA",
            "98101");

        Assert.AreEqual<string>("123 Main Street", address.StreetAddress);
        Assert.AreEqual<string>("Seattle", address.City);
        Assert.AreEqual<string>("WA", address.State);
        Assert.AreEqual<string>("98101", address.Zip);
    }

    [TestMethod]
    public void ConstructorNullStreetAddressThrowsArgumentNullException()
    {
        var exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Address(null!, "Seattle", "WA", "98101"));

        Assert.AreEqual<string>("streetAddress", exception.ParamName);
    }

    [TestMethod]
    public void ConstructorNullCityThrowsArgumentNullException()
    {
        var exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Address("123 Main Street", null!, "WA", "98101"));

        Assert.AreEqual<string>("city", exception.ParamName);
    }

    [TestMethod]
    public void ConstructorNullStateThrowsArgumentNullException()
    {
        var exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Address("123 Main Street", "Seattle", null!, "98101"));

        Assert.AreEqual<string>("state", exception.ParamName);
    }

    [TestMethod]
    public void ConstructorNullZipThrowsArgumentNullException()
    {
        var exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Address("123 Main Street", "Seattle", "WA", null!));

        Assert.AreEqual<string>("zip", exception.ParamName);
    }
}