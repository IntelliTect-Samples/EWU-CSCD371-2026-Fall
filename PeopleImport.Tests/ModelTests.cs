using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PeopleImport.Tests;

[TestClass]
public class ModelTests
{
    [TestMethod]
    [DataRow(null, "Lovelace", "ada@example.com", "firstName")]
    [DataRow("Ada", null, "ada@example.com", "lastName")]
    [DataRow("Ada", "Lovelace", null, "emailAddress")]
    public void Person_WithNullRequiredValue_IdentifiesArgument(
        string? firstName, string? lastName, string? emailAddress, string parameter)
    {
        Address address = new("1 Main St", "Spokane", "WA", "00123");

        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Person(1, firstName!, lastName!, emailAddress!, address));

        Assert.AreEqual<string>(parameter, exception.ParamName);
    }

    [TestMethod]
    public void Person_WithNullAddress_IdentifiesAddressArgument()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Person(1, "Ada", "Lovelace", "ada@example.com", null!));

        Assert.AreEqual<string>("address", exception.ParamName);
    }

    [TestMethod]
    [DataRow(null, "Spokane", "WA", "00123", "streetAddress")]
    [DataRow("1 Main St", null, "WA", "00123", "city")]
    [DataRow("1 Main St", "Spokane", null, "00123", "state")]
    [DataRow("1 Main St", "Spokane", "WA", null, "zip")]
    public void Address_WithNullRequiredValue_IdentifiesArgument(
        string? streetAddress, string? city, string? state, string? zip, string parameter)
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Address(streetAddress!, city!, state!, zip!));

        Assert.AreEqual<string>(parameter, exception.ParamName);
    }
}
