using Assignment;

namespace Resilience.Tests;

using Microsoft.VisualStudio.TestTools.UnitTesting;
[TestClass]
public sealed class PersonTester
{
    [TestMethod]
    public void Constructor_CreatesPerson_WhenArgumentsValid()
    {
        Address address =
            new("123 Main", "Cheney", "WA", "99004");

        Person person =
            new(
                1,
                "John",
                "Doe",
                "john@test.com",
                address);

        Assert.AreEqual(1, person.ID);
        Assert.AreEqual("John", person.FirstName);
        Assert.AreEqual("Doe", person.LastName);
        Assert.AreEqual("john@test.com", person.EmailAddress);
        Assert.AreSame(address, person.Address);
    }
    [TestMethod]
    public void Constructor_Throws_WhenIdIsZero()
    {
        Address address =
        new(
            "123 Main",
            "Cheney",
            "WA",
            "99004");

        try
        {
            _ = new Person(
                0,
                "John",
                "Doe",
                "john@test.com",
                address);

            Assert.Fail(
                "Expected ArgumentOutOfRangeException.");
        }
        catch (ArgumentOutOfRangeException ex)
        {
            Assert.AreEqual(
                "id",
                ex.ParamName);
        }
    }
    [TestMethod]
    public void Constructor_Throws_WhenFirstNameNull()
    {
        Address address =
            new(
                "123 Main St",
                "Cheney",
                "WA",
                "99004");

        try
        {
            _ = new Person(
                1,
                null!,
                "Doe",
                "john@test.com",
                address);

            Assert.Fail(
                "Expected ArgumentNullException.");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual(
                "firstName",
                ex.ParamName);
        }
    }
    [TestMethod]
    public void Constructor_Throws_WhenLastNameNull()
    {
        Address address =
            new(
                "123 Main St",
                "Cheney",
                "WA",
                "99004");

        try
        {
            _ = new Person(
                1,
                "John",
                null!,
                "john@test.com",
                address);

            Assert.Fail(
                "Expected ArgumentNullException.");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual(
                "lastName",
                ex.ParamName);
        }
    }
    [TestMethod]
    public void Constructor_Throws_WhenEmailNull()
    {
        Address address =
            new(
                "123 Main St",
                "Cheney",
                "WA",
                "99004");

        try
        {
            _ = new Person(
                1,
                "John",
                "Doe",
                null!,
                address);

            Assert.Fail(
                "Expected ArgumentNullException.");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual(
                "email",
                ex.ParamName);
        }
    }
    [TestMethod]
    public void Constructor_Throws_WhenAddressNull()
    {
        try
        {
            _ = new Person(
                1,
                "John",
                "Doe",
                "john@test.com",
                null!);

            Assert.Fail(
                "Expected ArgumentNullException.");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual(
                "address",
                ex.ParamName);
        }
    }
}
