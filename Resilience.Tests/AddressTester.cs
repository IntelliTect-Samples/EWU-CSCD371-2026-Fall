using Assignment;

namespace Resilience.Tests;

[TestClass]
public class AddressTester
{
    [TestMethod]
    public void Constructor_CreatesAddress_WhenArgumentsValid()
    {
        Address address =
            new(
                "123 Main St",
                "Cheney",
                "WA",
                "99004");

        Assert.AreEqual(
            "123 Main St",
            address.StreetAddress);

        Assert.AreEqual(
            "Cheney",
            address.City);

        Assert.AreEqual(
            "WA",
            address.State);

        Assert.AreEqual(
            "99004",
            address.Zip);
    }
    [TestMethod]
    public void Constructor_Throws_WhenStreetAddressNull()
    {
        try
        {
            _ = new Address(
                null!,
                "Cheney",
                "WA",
                "99004");

            Assert.Fail(
                "Expected ArgumentNullException.");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual(
                "streetAddress",
                ex.ParamName);
        }
    }
    [TestMethod]
    public void Constructor_Throws_WhenCityNull()
    {
        try
        {
            _ = new Address(
                "123 Main",
                null!,
                "WA",
                "99004");

            Assert.Fail(
                "Expected ArgumentNullException.");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual(
                "city",
                ex.ParamName);
        }
    }
    [TestMethod]
    public void Constructor_Throws_WhenStateNull()
    {
        try
        {
            _ = new Address(
                "123 Main",
                "Cheney",
                null!,
                "99004");

            Assert.Fail(
                "Expected ArgumentNullException.");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual(
                "state",
                ex.ParamName);
        }
    }
    [TestMethod]
    public void Constructor_Throws_WhenZipNull()
    {
        try
        {
            _ = new Address(
                "123 Main",
                "Cheney",
                "WA",
                null!);

            Assert.Fail(
                "Expected ArgumentNullException.");
        }
        catch (ArgumentNullException ex)
        {
            Assert.AreEqual(
                "zip",
                ex.ParamName);
        }
    }
}
