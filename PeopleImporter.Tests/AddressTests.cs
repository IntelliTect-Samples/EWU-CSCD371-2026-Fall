using Assignment;

namespace PeopleImporter.Tests;

[TestClass]
public sealed class AddressTests
{
    [TestMethod]
    public void Constructor_ValidArguments_SetsProperties()
    {
        Address address = new(
            "123 Main St",
            "Anytown",
            "CA",
            "12345");

        Assert.AreEqual("123 Main St", address.StreetAddress);
        Assert.AreEqual("Anytown", address.City);
        Assert.AreEqual("CA", address.State);
        Assert.AreEqual("12345", address.Zip);
    }

    [TestMethod]
    public void Constructor_NullStreetAddress_ThrowsArgumentNullException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new Address(null!, "Anytown", "CA", "12345"));

        Assert.AreEqual("streetAddress", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullCity_ThrowsArgumentNullException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new Address("123 Main St", null!, "CA", "12345"));

        Assert.AreEqual("city", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullState_ThrowsArgumentNullException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new Address("123 Main St", "Anytown", null!, "12345"));

        Assert.AreEqual("state", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_NullZip_ThrowsArgumentNullException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => new Address("123 Main St", "Anytown", "CA", null!));

        Assert.AreEqual("zip", exception.ParamName);
    }
}