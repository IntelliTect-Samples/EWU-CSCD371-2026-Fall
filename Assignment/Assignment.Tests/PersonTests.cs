using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class PersonTests
{
    private static readonly Address ValidAddress = new("1 Main St", "Spokane", "WA", "99201");

    [TestMethod]
    public void Address_StoresValues()
    {
        Assert.AreEqual<string>("1 Main St", ValidAddress.StreetAddress);
        Assert.AreEqual<string>("Spokane", ValidAddress.City);
        Assert.AreEqual<string>("WA", ValidAddress.State);
        Assert.AreEqual<string>("99201", ValidAddress.Zip);
    }

    [TestMethod]
    public void Address_NullArguments_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => new Address(null!, "c", "s", "z"));
        Assert.Throws<ArgumentNullException>(() => new Address("a", null!, "s", "z"));
        Assert.Throws<ArgumentNullException>(() => new Address("a", "c", null!, "z"));
        Assert.Throws<ArgumentNullException>(() => new Address("a", "c", "s", null!));
    }

    [TestMethod]
    public void Person_StoresValues()
    {
        Person person = new("Ada", "Lovelace", ValidAddress, "ada@example.com");

        Assert.AreEqual<string>("Ada", person.FirstName);
        Assert.AreEqual<string>("Lovelace", person.LastName);
        Assert.AreEqual<string>("ada@example.com", person.EmailAddress);
        Assert.AreSame(ValidAddress, person.Address);
    }

    [TestMethod]
    public void Person_NullArguments_Throw()
    {
        Assert.Throws<ArgumentNullException>(() => new Person(null!, "l", ValidAddress, "e"));
        Assert.Throws<ArgumentNullException>(() => new Person("f", null!, ValidAddress, "e"));
        Assert.Throws<ArgumentNullException>(() => new Person("f", "l", null!, "e"));
        Assert.Throws<ArgumentNullException>(() => new Person("f", "l", ValidAddress, null!));
    }
}
