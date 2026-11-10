using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class PersonTests
{
    [TestMethod]
    [DataRow(null, "Lovelace", "ada@example.com", "firstName")]
    [DataRow("Ada", null, "ada@example.com", "lastName")]
    [DataRow("Ada", "Lovelace", null, "emailAddress")]
    public void Constructor_WithNullNameOrEmail_IdentifiesInvalidArgument(
        string? firstName, string? lastName, string? emailAddress, string parameter)
    {
        Address address = new("1 Main St", "Spokane", "WA", "99201");

        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Person(firstName!, lastName!, emailAddress!, address));

        Assert.AreEqual<string>(parameter, exception.ParamName);
    }

    [TestMethod]
    public void Constructor_WithNullAddress_ThrowsArgumentNullException()
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Person("Ada", "Lovelace", "ada@example.com", null!));

        Assert.AreEqual<string>("address", exception.ParamName);
    }
}
