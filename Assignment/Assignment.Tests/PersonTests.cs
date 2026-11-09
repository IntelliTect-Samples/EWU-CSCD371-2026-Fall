using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class PersonTests
{
    [TestMethod]
    public void Constructor_ValidArguments_SetsProperties()
    {
        var address = new Address(
            "123 Main Street",
            "Seattle",
            "WA",
            "98101");

        var person = new Person(
            "John",
            "Smith",
            address,
            "john.smith@example.com");

        Assert.AreEqual<string>("John", person.FirstName);
        Assert.AreEqual<string>("Smith", person.LastName);
        Assert.AreSame(address, person.Address);
        Assert.AreEqual<string>(
            "john.smith@example.com",
            person.EmailAddress);
    }

    [TestMethod]
    public void Constructor_NullFirstName_ThrowsArgumentNullException()
    {
        var address = new Address("123 Main Street", "Seattle", "WA", "98101");

        var exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Person(null!, "Smith", address, "john@example.com"));

        Assert.AreEqual<string>("firstName", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullLastName_ThrowsArgumentNullException()
    {
        var address = new Address("123 Main Street", "Seattle", "WA", "98101");

        var exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Person("John", null!, address, "john@example.com"));

        Assert.AreEqual<string>("lastName", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullAddress_ThrowsArgumentNullException()
    {
        var exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Person("John", "Smith", null!, "john@example.com"));

        Assert.AreEqual<string>("address", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullEmailAddress_ThrowsArgumentNullException()
    {
        var address = new Address("123 Main Street", "Seattle", "WA", "98101");

        var exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Person("John", "Smith", address, null!));

        Assert.AreEqual<string>("emailAddress", exception.ParamName);
    }
}