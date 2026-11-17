using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment.Tests;

[TestClass]
public class PersonRecordParserTests
{
    private const string ValidLine =
        "1,John,Doe,john@example.com,123 Main St,Spokane,WA,99201";

    [TestMethod]
    public void Parse_ValidRecord_ReturnsPerson()
    {
        Person person = PersonRecordParser.Parse(ValidLine);

        Assert.AreEqual(1, person.Id);
        Assert.AreEqual("John", person.FirstName);
        Assert.AreEqual("Doe", person.LastName);
        Assert.AreEqual("john@example.com", person.EmailAddress);
        Assert.AreEqual("123 Main St", person.Address.StreetAddress);
        Assert.AreEqual("Spokane", person.Address.City);
        Assert.AreEqual("WA", person.Address.State);
        Assert.AreEqual("99201", person.Address.Zip);
    }

    [TestMethod]
    public void Parse_Whitespace_TrimsFields()
    {
        string line =
            " 1 , John , Doe , john@example.com , 123 Main St , Spokane , WA , 99201 ";

        Person person = PersonRecordParser.Parse(line);

        Assert.AreEqual(1, person.Id);
        Assert.AreEqual("John", person.FirstName);
        Assert.AreEqual("Doe", person.LastName);
        Assert.AreEqual("Spokane", person.Address.City);
    }

    [TestMethod]
    [DataRow("1,John,Doe")]
    [DataRow("1,John,Doe,john@example.com,123 Main St,Spokane,WA,99201,Extra")]
    public void Parse_WrongFieldCount_ThrowsFormatException(string line)
    {
        Assert.ThrowsExactly<FormatException>(
            () => PersonRecordParser.Parse(line));
    }

    [TestMethod]
    public void Parse_NonNumericId_ThrowsFormatException()
    {
        string line =
            "abc,John,Doe,john@example.com,123 Main St,Spokane,WA,99201";

        Assert.ThrowsExactly<FormatException>(
            () => PersonRecordParser.Parse(line));
    }

    [TestMethod]
    [DataRow("0")]
    [DataRow("-5")]
    public void Parse_NonPositiveId_ThrowsFormatException(string id)
    {
        string line =
            $"{id},John,Doe,john@example.com,123 Main St,Spokane,WA,99201";

        Assert.ThrowsExactly<FormatException>(
            () => PersonRecordParser.Parse(line));
    }

    [TestMethod]
    public void Parse_OverflowId_ThrowsOverflowException()
    {
        string line =
            "999999999999,John,Doe,john@example.com,123 Main St,Spokane,WA,99201";

        Assert.ThrowsExactly<OverflowException>(
            () => PersonRecordParser.Parse(line));
    }

    [TestMethod]
    [DataRow("1,,Doe,john@example.com,123 Main St,Spokane,WA,99201")]
    [DataRow("1,John,,john@example.com,123 Main St,Spokane,WA,99201")]
    [DataRow("1,John,Doe,,123 Main St,Spokane,WA,99201")]
    [DataRow("1,John,Doe,john@example.com,123 Main St,,WA,99201")]
    public void Parse_MissingRequiredField_ThrowsFormatException(string line)
    {
        Assert.ThrowsExactly<FormatException>(
            () => PersonRecordParser.Parse(line));
    }

    [TestMethod]
    public void Parse_NullLine_ThrowsArgumentNullException()
    {
        var exception =
            Assert.ThrowsExactly<ArgumentNullException>(
                () => PersonRecordParser.Parse(null!));

        Assert.AreEqual("rawLine", exception.ParamName);
    }
}