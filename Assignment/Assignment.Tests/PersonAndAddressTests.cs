using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class PersonAndAddressTests
{
    private static readonly Address ValidAddress = new("1 Main St", "Spokane", "WA", "99201");

    [TestMethod]
    public void Address_ValidArguments_SetsProperties()
    {
        var address = new Address("1 Main St", "Spokane", "WA", "99201");

        Assert.AreEqual<string>("1 Main St", address.StreetAddress);
        Assert.AreEqual<string>("Spokane", address.City);
        Assert.AreEqual<string>("WA", address.State);
        Assert.AreEqual<string>("99201", address.Zip);
    }

    [TestMethod]
    public void Address_NullArgument_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address(null!, "Spokane", "WA", "99201"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address("1 Main St", null!, "WA", "99201"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address("1 Main St", "Spokane", null!, "99201"));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Address("1 Main St", "Spokane", "WA", null!));
    }

    [TestMethod]
    public void Person_ValidArguments_SetsProperties()
    {
        var person = new Person("Ada", "Lovelace", "ada@example.com", ValidAddress);

        Assert.AreEqual<string>("Ada", person.FirstName);
        Assert.AreEqual<string>("Lovelace", person.LastName);
        Assert.AreEqual<string>("ada@example.com", person.EmailAddress);
        Assert.AreSame(ValidAddress, person.Address);
    }

    [TestMethod]
    public void Person_NullArgument_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person(null!, "Lovelace", "ada@example.com", ValidAddress));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("Ada", null!, "ada@example.com", ValidAddress));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("Ada", "Lovelace", null!, ValidAddress));
        Assert.ThrowsExactly<ArgumentNullException>(() => new Person("Ada", "Lovelace", "ada@example.com", null!));
    }
}