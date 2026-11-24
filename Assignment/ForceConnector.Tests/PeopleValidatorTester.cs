using Microsoft.VisualStudio.TestTools.UnitTesting;
using ForceConnector.Core.Models;
using ForceConnector.Core.Services;
namespace ForceConnector.Tests;

[TestClass]
public class PeopleValidatorTester
{
    private readonly PersonValidator _validator =
        new();
    [TestMethod]
    public void IsValidReturnsFalseWhenFirstNameBlank()
    {
        Person person = new()
        {
            FirstName = "",
            LastName = "Edwards",
            EmailAddress = "mateo@test.com",
            ZipCode = "99201"
        };

        Assert.IsFalse(
            _validator.IsValid(person));
    }

    [TestMethod]
    public void GetErrorsIdentifiesInvalidEmail()
    {
        Person person = new()
        {
            FirstName = "Mateo",
            LastName = "Edwards",
            EmailAddress = "test@",
            ZipCode = "99201"
        };

        IReadOnlyList<string> errors = _validator.GetErrors(person);

        Assert.HasCount(1, errors);
        StringAssert.Contains(errors[0], "Email address is invalid");
    }
    [TestMethod]
    public void IsValidReturnsFalseWhenEmailInvalid()
    {
        Person person = new()
        {
            FirstName = "Mateo",
            LastName = "Edwards",
            EmailAddress = "invalid",
            ZipCode = "99201"
        };

        Assert.IsFalse(
            _validator.IsValid(person));
    }
    [TestMethod]
    public void IsValidReturnsFalseWhenZipInvalid()
    {
        Person person = new()
        {
            FirstName = "Mateo",
            LastName = "Edwards",
            EmailAddress = "mateo@test.com",
            ZipCode = "ABC"
        };

        Assert.IsFalse(
            _validator.IsValid(person));
    }
    [TestMethod]
    public void IsValidReturnsTrueWhenPersonValid()
    {
        Person person = new()
        {
            FirstName = "Mateo",
            LastName = "Edwards",
            EmailAddress = "mateo@test.com",
            ZipCode = "99201"
        };

        Assert.IsTrue(
            _validator.IsValid(person));
    }
}
