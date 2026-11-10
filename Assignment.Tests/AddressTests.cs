using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class AddressTests
{
    [TestMethod]
    [DataRow(null, "Spokane", "WA", "99201", "streetAddress")]
    [DataRow("1 Main St", null, "WA", "99201", "city")]
    [DataRow("1 Main St", "Spokane", null, "99201", "state")]
    [DataRow("1 Main St", "Spokane", "WA", null, "zip")]
    public void Constructor_WithNullProperty_IdentifiesInvalidArgument(
        string? streetAddress, string? city, string? state, string? zip, string parameter)
    {
        ArgumentNullException exception = Assert.ThrowsExactly<ArgumentNullException>(
            () => new Address(streetAddress!, city!, state!, zip!));

        Assert.AreEqual<string>(parameter, exception.ParamName);
    }
}
