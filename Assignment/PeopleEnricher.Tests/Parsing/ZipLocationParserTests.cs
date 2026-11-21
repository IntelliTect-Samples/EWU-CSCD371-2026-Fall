using PeopleEnricher;

namespace PeopleEnricher.Tests;

[TestClass]
public sealed class ZipLocationParserTests
{
    [TestMethod]
    public void ParseValidResponseReturnsLocation()
    {
        ZipLocation location =
            ZipLocationParser.Parse("\"SPOKANE\",\"WA\"");

        Assert.AreEqual("SPOKANE", location.City);
        Assert.AreEqual("WA", location.State);
    }

    [TestMethod]
    public void ParseResponseWithCommaPreservesCity()
    {
        ZipLocation location =
            ZipLocationParser.Parse("\"WASHINGTON, D.C.\",\"DC\"");

        Assert.AreEqual("WASHINGTON, D.C.", location.City);
        Assert.AreEqual("DC", location.State);
    }

    [TestMethod]
    public void ParseResponseWithEscapedQuotePreservesCity()
    {
        ZipLocation location =
            ZipLocationParser.Parse("\"TEST \"\"CITY\"\"\",\"WA\"");

        Assert.AreEqual("TEST \"CITY\"", location.City);
    }

    [TestMethod]
    public void ParseResponseWithLineEndingSucceeds()
    {
        ZipLocation location =
            ZipLocationParser.Parse("\"SPOKANE\",\"WA\"\r\n");

        Assert.AreEqual("SPOKANE", location.City);
    }

    [TestMethod]
    public void ParseBadDataThrowsInvalidDataException()
    {
        Assert.ThrowsExactly<InvalidDataException>(
            () => ZipLocationParser.Parse("BAD_DATA"));
    }

    [TestMethod]
    public void ParseEmptyResponseThrowsInvalidDataException()
    {
        Assert.ThrowsExactly<InvalidDataException>(
            () => ZipLocationParser.Parse(string.Empty));
    }

    [TestMethod]
    public void ParseMissingCityThrowsInvalidDataException()
    {
        Assert.ThrowsExactly<InvalidDataException>(
            () => ZipLocationParser.Parse("\"\",\"WA\""));
    }

    [TestMethod]
    public void ParseInvalidStateThrowsInvalidDataException()
    {
        Assert.ThrowsExactly<InvalidDataException>(
            () => ZipLocationParser.Parse("\"SPOKANE\",\"WASHINGTON\""));
    }

    [TestMethod]
    public void ParseExtraColumnThrowsInvalidDataException()
    {
        Assert.ThrowsExactly<InvalidDataException>(
            () => ZipLocationParser.Parse("\"SPOKANE\",\"WA\",\"USA\""));
    }

    [TestMethod]
    public void ParseUnquotedResponseThrowsInvalidDataException()
    {
        Assert.ThrowsExactly<InvalidDataException>(
            () => ZipLocationParser.Parse("SPOKANE,WA"));
    }
}