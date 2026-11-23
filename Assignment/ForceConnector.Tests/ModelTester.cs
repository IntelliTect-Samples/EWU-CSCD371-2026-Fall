using ForceConnector.Core.Models;

namespace ForceConnector.Tests;

[TestClass]
public sealed class ModelTests
{
    [TestMethod]
    public void Person_StoresValues()
    {
        Person person = new()
        {
            FirstName = "Mateo",
            LastName = "Edwards",
            EmailAddress = "mateo@test.com",
            ZipCode = "99201"
        };

        Assert.AreEqual("Mateo", person.FirstName);
        Assert.AreEqual("Edwards", person.LastName);
        Assert.AreEqual("mateo@test.com", person.EmailAddress);
        Assert.AreEqual("99201", person.ZipCode);
    }

    [TestMethod]
    public void Location_StoresValues()
    {
        Location location = new()
        {
            City = "Spokane",
            State = "WA"
        };

        Assert.AreEqual("Spokane", location.City);
        Assert.AreEqual("WA", location.State);
    }

    [TestMethod]
    public void Rejection_StoresValues()
    {
        Rejection rejection = new()
        {
            EmailAddress = "test@test.com",
            Reason = "Bad Zip"
        };

        Assert.AreEqual(
            "test@test.com",
            rejection.EmailAddress);

        Assert.AreEqual(
            "Bad Zip",
            rejection.Reason);
    }
}
