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
    public void ParsePeopleSkipsInvalidRows()
    {
        string csv =
            """
            FirstName,LastName,EmailAddress,ZipCode
            Mateo,Edwards,mateo@test.com,99201
            BrokenRow
            """;

        CsvParser parser = new();

        IReadOnlyList<Person> people =
            parser.ParsePeople(csv);

        Assert.HasCount(1, people);
    }
}