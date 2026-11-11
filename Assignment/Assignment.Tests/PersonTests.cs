using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Assignment.Tests;

[TestClass]
public class PersonTests
{
    private readonly Address _address = new("1234 street Ave.", "Village City", "State", "54321");
    private const string _firstName = "John";
    private const string _lastName = "Smith";
    private const string _email = "email@website.org";


    [TestMethod]
    [DataRow(_firstName, _lastName, null)]
    [DataRow(null, _lastName, _email)]
    [DataRow(_firstName, null, _email)]
    public void Constructor_NullStringArgument_Throws(string firstName, string lastName, string email)
    {
        Person p;
        Assert.Throws<ArgumentNullException>(() => p = new(firstName, lastName, email, _address));
    }

    [TestMethod]
    public void Constructor_NullAddress_Throws()
    {
        Person p;
        Assert.Throws<ArgumentNullException>(() => p = new("firstName", "lastName", "email", null!));
    }

    [TestMethod]
    public void Constructor_ValidArgs_CorrectValues()
    {
        Person p = new(_firstName, _lastName, _email, _address);

        Assert.AreEqual(_firstName, p.FirstName);
        Assert.AreEqual(_lastName, p.LastName);
        Assert.AreEqual(_email, p.EmailAddress);
        Assert.AreEqual(_address, p.Address);
    }
}