using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment9.Tests;

[TestClass]
public sealed class PersonValidatorTests
{
    [TestMethod]
    [DataRow("Ada,Lovelace,ada@example.com,99201")]
    [DataRow("Mary-Jane,O'Brien,mj.obrien+tag@mail.example.org,01234")]
    [DataRow("\"José\",\"St. John\",jose@example.com,99201")]
    public void Validate_ValidRow_ReturnsPerson(string line)
    {
        PersonValidationResult result = PersonValidator.Validate(line, rowNumber: 2);

        Assert.IsNotNull(result.Person);
        Assert.IsNull(result.Rejection);
    }

    [TestMethod]
    public void Validate_ValidRow_KeepsValuesExactly()
    {
        PersonValidationResult result = PersonValidator.Validate("Ada,Lovelace,ada@example.com,99201", rowNumber: 2);

        Assert.AreEqual(new Person("Ada", "Lovelace", "ada@example.com", "99201"), result.Person);
    }

    [TestMethod]
    [DataRow("Ada,\"Lovelace,ada@example.com,99201", RejectionReasons.MalformedRow)]
    [DataRow("Ada,Lovelace,ada@example.com", RejectionReasons.WrongFieldCount)]
    [DataRow("Ada,Lovelace,ada@example.com,99201,extra", RejectionReasons.WrongFieldCount)]
    [DataRow("BAD_DATA", RejectionReasons.WrongFieldCount)]
    [DataRow(",Lovelace,ada@example.com,99201", RejectionReasons.InvalidFirstName)]
    [DataRow("Ad4,Lovelace,ada@example.com,99201", RejectionReasons.InvalidFirstName)]
    [DataRow(" Ada,Lovelace,ada@example.com,99201", RejectionReasons.InvalidFirstName)]
    [DataRow("Ada,,ada@example.com,99201", RejectionReasons.InvalidLastName)]
    [DataRow("Ada,Lovelace ,ada@example.com,99201", RejectionReasons.InvalidLastName)]
    [DataRow("Ada,Lovelace,not-an-email,99201", RejectionReasons.InvalidEmail)]
    [DataRow("Ada,Lovelace,ada@example,99201", RejectionReasons.InvalidEmail)]
    [DataRow("Ada,Lovelace,ada..x@example.com,99201", RejectionReasons.InvalidEmail)]
    [DataRow("Ada,Lovelace,,99201", RejectionReasons.InvalidEmail)]
    [DataRow("Ada,Lovelace,ada@example.com,9920", RejectionReasons.InvalidZipCode)]
    [DataRow("Ada,Lovelace,ada@example.com,ABCDE", RejectionReasons.InvalidZipCode)]
    [DataRow("Ada,Lovelace,ada@example.com,99201-1234", RejectionReasons.InvalidZipCode)]
    [DataRow("Ada,Lovelace,ada@example.com,", RejectionReasons.InvalidZipCode)]
    public void Validate_InvalidRow_ReturnsRejectionWithReason(string line, string expectedReason)
    {
        PersonValidationResult result = PersonValidator.Validate(line, rowNumber: 7);

        Assert.IsNull(result.Person);
        Assert.IsNotNull(result.Rejection);
        Assert.AreEqual(expectedReason, result.Rejection.Reason);
        Assert.Contains("Row 7", result.Rejection.Subject);
    }
}
