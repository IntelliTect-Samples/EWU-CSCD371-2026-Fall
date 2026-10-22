using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class EmployeeTests
{
    [TestMethod]
    public void Constructor_NullName_ThrowsArgumentNullException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() => new Employee(null!, ["ValidEmail"]));
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpaceEmployer_StoresNull(string? employer)
    {
        Employee employee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"], employer: employer);

        Assert.IsNull(employee.Employer);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpaceJobTitle_StoresNull(string? jobTitle)
    {
        Employee employee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"], jobTitle: jobTitle);

        Assert.IsNull(employee.JobTitle);
    }

    [TestMethod]
    public void Equals_IdenticalSeparateInstances_ReturnsTrue()
    {
        Employee originalEmployee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            employer: "ValidEmployer", jobTitle: "ValidJobTitle");
        Employee comparedEmployee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            employer: "ValidEmployer", jobTitle: "ValidJobTitle");

        Assert.IsTrue(Equals(originalEmployee, comparedEmployee));
    }

    [TestMethod]
    [DataRow("DifferentEmployer", "ValidJobTitle")]
    [DataRow("ValidEmployer", "DifferentJobTitle")]
    public void Equals_OneEmployeeComponentDiffers_ReturnsFalse(string employer, string jobTitle)
    {
        Employee originalEmployee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            employer: employer, jobTitle: jobTitle);
        Employee comparedEmployee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            employer: "ValidEmployer", jobTitle: "ValidJobTitle");

        Assert.IsFalse(Equals(originalEmployee, comparedEmployee));
    }

    [TestMethod]
    public void GetHashCode_IdenticalSeparateInstances_ReturnsEqualHashCodes()
    {
        Employee originalEmployee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            employer: "ValidEmployer", jobTitle: "ValidJobTitle");
        Employee comparedEmployee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            employer: "ValidEmployer", jobTitle: "ValidJobTitle");

        Assert.AreEqual(originalEmployee.GetHashCode(), comparedEmployee.GetHashCode());
    }

    [TestMethod]
    public void ToString_NullEmployerAndJobTitle_ReturnsExpectedString()
    {
        Employee employee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"]);
        string expectedOutput = $"Id: {employee.Id}{Environment.NewLine}"
            + $"Name: ValidFirst ValidLast{Environment.NewLine}"
            + $"Emails: ValidEmail{Environment.NewLine}";

        Assert.AreEqual(expectedOutput, employee.ToString());
    }

    [TestMethod]
    public void ToString_ProvidedEmployerAndJobTitle_ReturnsExpectedString()
    {
        Employee employee = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            employer: "ValidEmployer", jobTitle: "ValidJobTitle");
        string expectedOutput = $"Id: {employee.Id}{Environment.NewLine}"
            + $"Name: ValidFirst ValidLast{Environment.NewLine}"
            + $"Emails: ValidEmail{Environment.NewLine}"
            + $"Employer: ValidEmployer{Environment.NewLine}"
            + $"JobTitle: ValidJobTitle{Environment.NewLine}";

        Assert.AreEqual(expectedOutput, employee.ToString());
    }
}
