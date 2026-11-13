using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class AddressTests
{
    [TestMethod]
    public void Address_NullStreetAddressThrowsException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new Address(null!, "Spokane", "WA", "99201"));
    }

    [TestMethod]
    public void Address_NullCityThrowsException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new Address("123 Main St", null!, "WA", "99201"));
    }

    [TestMethod]
    public void Address_NullStateThrowsException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new Address("123 Main St", "Spokane", null!, "99201"));
    }

    [TestMethod]
    public void Address_NullZipThrowsException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new Address("123 Main St", "Spokane", "WA", null!));
    }
}