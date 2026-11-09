using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class PersonAndAddressTests
{
    [TestMethod]
    public void Address_NullArgument_Throws()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address(null!, "City", "ST", "12345"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address("Street", null!, "ST", "12345"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address("Street", "City", null!, "12345"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address("Street", "City", "ST", null!));
    }

    [TestMethod]
    public void Person_NullArgument_Throws()
    {
        Address address = new("Street", "City", "ST", "12345");

        Assert.ThrowsExactly<ArgumentNullException>(() => new Person(null!, "Last", address, "a@b.com"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("First", null!, address, "a@b.com"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("First", "Last", null!, "a@b.com"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("First", "Last", address, null!));
    }
}
