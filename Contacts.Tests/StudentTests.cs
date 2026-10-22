using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Contacts.Tests;

[TestClass]
public class StudentTests
{
    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpaceSchoolName_StoresNull(string? schoolName)
    {
        Student student = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"], schoolName: schoolName);

        Assert.IsNull(student.SchoolName);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" ")]
    [DataRow("\t")]
    public void Constructor_NullOrWhiteSpaceMajor_StoresNull(string? major)
    {
        Student student = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"], major: major);

        Assert.IsNull(student.Major);
    }

    [TestMethod]
    public void Equals_IdenticalSeparateInstances_ReturnsTrue()
    {
        Student originalStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            schoolName: "ValidSchool", major: "ValidMajor");
        Student comparedStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            schoolName: "ValidSchool", major: "ValidMajor");

        Assert.IsTrue(Equals(originalStudent, comparedStudent));
    }

    [TestMethod]
    [DataRow("DifferentSchool", "ValidMajor")]
    [DataRow("ValidSchool", "DifferentMajor")]
    public void Equals_OneStudentComponentDiffers_ReturnsFalse(string schoolName, string major)
    {
        Student originalStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            schoolName: schoolName, major: major);
        Student comparedStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            schoolName: "ValidSchool", major: "ValidMajor");

        Assert.IsFalse(Equals(originalStudent, comparedStudent));
    }

    [TestMethod]
    public void GetHashCode_IdenticalSeparateInstances_ReturnsEqualHashCodes()
    {
        Student originalStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            schoolName: "ValidSchool", major: "ValidMajor");
        Student comparedStudent = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            schoolName: "ValidSchool", major: "ValidMajor");

        Assert.AreEqual(originalStudent.GetHashCode(), comparedStudent.GetHashCode());
    }

    [TestMethod]
    public void ToString_ProvidedSchoolNameAndMajor_ReturnsExpectedString()
    {
        Student student = new(new FullName("ValidFirst", "ValidLast"), ["ValidEmail"],
            schoolName: "ValidSchool", major: "ValidMajor");
        string expectedOutput = $"Id: {student.Id}{Environment.NewLine}"
            + $"Name: ValidFirst ValidLast{Environment.NewLine}"
            + $"Emails: ValidEmail{Environment.NewLine}"
            + $"SchoolName: ValidSchool{Environment.NewLine}"
            + $"Major: ValidMajor{Environment.NewLine}";

        Assert.AreEqual(expectedOutput, student.ToString());
    }
}