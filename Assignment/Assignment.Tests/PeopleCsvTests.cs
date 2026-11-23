using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment9.Tests;

[TestClass]
public sealed class PeopleCsvTests
{
    private static readonly string[] expectedFirstNames = ["Ada", "Grace"];
    private static readonly string[] expectedReasons = [RejectionReasons.InvalidEmail, RejectionReasons.InvalidZipCode];

    [TestMethod]
    public void Parse_ValidCsv_ReturnsEveryPerson()
    {
        PeopleParseResult result = PeopleCsv.Parse(SampleData.PeopleCsv);

        Assert.HasCount(3, result.People);
        Assert.IsEmpty(result.Rejections);
        Assert.AreEqual("Ada", result.People[0].FirstName);
    }

    [TestMethod]
    public void Parse_CrLfLineEndingsBlankLinesAndBom_AreTolerated()
    {
        string csv = "﻿" + PeopleCsv.ExpectedHeader + "\r\nAda,Lovelace,ada@example.com,99201\r\n\r\n";

        PeopleParseResult result = PeopleCsv.Parse(csv);

        Assert.HasCount(1, result.People);
    }

    [TestMethod]
    public void Parse_SomeBadRows_RemovesOnlyThoseAndReportsWhy()
    {
        string csv = SampleData.Csv(
            "Ada,Lovelace,ada@example.com,99201",
            "Bad,Email,nope,99201",
            "Bad,Zip,bz@example.com,ABCDE",
            "Grace,Hopper,grace@example.com,98101");

        PeopleParseResult result = PeopleCsv.Parse(csv);

        CollectionAssert.AreEqual(expectedFirstNames, result.People.Select(p => p.FirstName).ToArray());
        CollectionAssert.AreEqual(
            expectedReasons,
            result.Rejections.Select(r => r.Reason).ToArray());
        Assert.Contains("Row 3", result.Rejections[0].Subject);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("\n\n")]
    [DataRow("BAD_DATA")]
    [DataRow("First,Last,Email,Zip\nAda,Lovelace,ada@example.com,99201")]
    [DataRow("FirstName,LastName,EmailAddress,ZipCode\n")]
    [DataRow("FirstName,LastName,EmailAddress,ZipCode\nBAD_DATA\nBAD_DATA")]
    public void Parse_InvalidResponse_Throws(string csv)
    {
        Assert.ThrowsExactly<InvalidResponseException>(() => PeopleCsv.Parse(csv));
    }
}
