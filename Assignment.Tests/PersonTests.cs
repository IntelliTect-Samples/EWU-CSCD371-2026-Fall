using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class PersonTests
{
    [TestMethod]
    public void Person_WhenFirstNameIsNull_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            CreatePerson(firstName: null!));
    }
    
    [TestMethod]
    public void Person_WhenLastNameIsNull_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            CreatePerson(lastName: null!));
    }
    
    [TestMethod]
    public void Person_WhenAddressIsNull_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            new Person(
                firstName: "firstName",
                lastName: "lastName",
                emailAddress: "emailAddress",
                null!));
    }
    
    [TestMethod]
    public void Person_WhenEmailAddressIsNull_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
            CreatePerson(emailAddress: null!));
    }
    
    private static Person CreatePerson(
        string firstName = "defaultFirstName",
        string lastName = "defaultLastName",
        string emailAddress = "defaultEmail",
        IAddress? address = null)
    {
        return new Person(
            firstName,
            lastName,
            emailAddress,
            address ?? CreateAddress());
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