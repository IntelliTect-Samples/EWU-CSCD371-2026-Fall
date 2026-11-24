using ForceConnector.Core.Models;
using ForceConnector.Core.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ForceConnector.Tests;

[TestClass]
public sealed class CsvParserTests
{
    [TestMethod]
    public void ParsePeopleReturnsPeople()
    {
        string csv =
            """
            FirstName,LastName,EmailAddress,ZipCode
            Mateo,Edwards,mateo@test.com,99201
            """;

        CsvParser parser = new();

        IReadOnlyList<Person> people =
            parser.ParsePeople(csv);

        Assert.HasCount(1, people);
    }

    [TestMethod]
    public void ParsePeopleThrowsForBadHeader()
    {
        CsvParser parser = new();

        Assert.Throws<InvalidDataException>(
            () => parser.ParsePeople(
                """
                Bad,Header
                Value,Value
                """));
    }

    [TestMethod]
    public void ParsePeopleReportsInvalidRows()
    {
        string csv =
            "FirstName,LastName,EmailAddress,ZipCode\r\n" +
            "Mateo,Edwards,mateo@test.com,99201\n" +
            "BrokenRow\r";

        CsvParser parser = new();

        CsvParseResult result =
            parser.ParsePeopleWithRejections(csv);

        Assert.HasCount(1, result.People);
        Assert.HasCount(1, result.Rejections);
        StringAssert.Contains(
            result.Rejections[0].Reason,
            "Expected 4 columns");
    }

    [TestMethod]
    public void ParsePeopleSupportsQuotedFields()
    {
        CsvParser parser = new();

        IReadOnlyList<Person> people = parser.ParsePeople(
            "FirstName,LastName,EmailAddress,ZipCode\n" +
            "\"Mateo, Jr.\",Edwards,\"mateo+test@example.com\",99201\n");

        Assert.AreEqual("Mateo, Jr.", people[0].FirstName);
        Assert.AreEqual("mateo+test@example.com", people[0].EmailAddress);
    }
}