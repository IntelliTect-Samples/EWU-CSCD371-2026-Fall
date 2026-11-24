using ForceConnector.Core.Interfaces;
using ForceConnector.Core.Models;
using ForceConnector.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ForceConnector.Tests;

[TestClass]
public sealed class ModelTests
{
    [TestMethod]
    public void PersonStoresValues()
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
    public void LocationStoresValues()
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
    public void RejectionStoresValues()
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
