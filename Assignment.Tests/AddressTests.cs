using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class AddressTests
{
    [TestMethod]
    public void Address_WhenStreetAddressIsNull_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            CreateAddress(streetAddress: null!));
    }

    [TestMethod]
    public void Address_WhenCityIsNull_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            CreateAddress(city: null!));
    }

    [TestMethod]
    public void Address_WhenStateIsNull_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            CreateAddress(state: null!));
    }
    
    [TestMethod]
    public void Address_WhenZipIsNull_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            CreateAddress(zip: null!));
    }

    private static Address CreateAddress(
        string streetAddress = "defaultAddress",
        string city = "defaultCity",
        string state = "defaultState",
        string zip = "defaultZip")
    {
        return new Address(
            streetAddress,
            city,
            state,
            zip);
    }
}