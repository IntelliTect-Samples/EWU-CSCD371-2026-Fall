using PeopleEnricher;

namespace PeopleEnricher.Tests;

[TestClass]
public sealed class PeopleCsvParserTests
{
    private const string Header =
        "FirstName,LastName,EmailAddress,ZipCode";

    [TestMethod]
    public void ParseValidCsvReturnsPerson()
    {
        string csv =
            $"{Header}\nJane,Doe,jane.doe@example.com,99201";

        PeopleCsvParseResult result = PeopleCsvParser.Parse(csv);

        Person person = result.People.Single();

        Assert.AreEqual("Jane", person.FirstName);
        Assert.AreEqual("Doe", person.LastName);
        Assert.AreEqual("jane.doe@example.com", person.EmailAddress);
        Assert.AreEqual("99201", person.ZipCode);

        Assert.IsEmpty(result.Rejections);
    }

    [TestMethod]
    public void ParseQuotedCommaPreservesField()
    {
        string csv =
            $"{Header}\nJane,\"Doe, Jr.\",jane@example.com,99201";

        PeopleCsvParseResult result = PeopleCsvParser.Parse(csv);

        Assert.AreEqual("Doe, Jr.", result.People.Single().LastName);
        Assert.IsEmpty(result.Rejections);
    }

    [TestMethod]
    public void ParseEscapedQuotePreservesField()
    {
        string csv =
            $"{Header}\nJane,\"Do\"\"e\",jane@example.com,99201";

        PeopleCsvParseResult result = PeopleCsvParser.Parse(csv);

        Assert.AreEqual("Do\"e", result.People.Single().LastName);
    }

    [TestMethod]
    public void ParseInvalidRowsRejectsOnlyThoseRows()
    {
        string csv =
            $"{Header}\n" +
            "Jane,Doe,jane@example.com,99201\n" +
            "John,Smith,invalid-email,99202\n" +
            "Alex,Green,alex@example.com,1234\n" +
            ",Brown,brown@example.com,99203";

        PeopleCsvParseResult result = PeopleCsvParser.Parse(csv);

        Assert.HasCount(1, result.People);
        Assert.HasCount(3, result.Rejections);

        Assert.AreEqual("Jane", result.People.Single().FirstName);
        Assert.IsTrue(result.Rejections.All(
            rejection => rejection.Stage == RejectionStage.PeopleCsv));
    }

    [TestMethod]
    public void ParseInvalidRowsReportsLineNumbersAndReasons()
    {
        string invalidRow =
            "John,Smith,invalid-email,99202";

        string csv = $"{Header}\n{invalidRow}";

        PeopleCsvParseResult result = PeopleCsvParser.Parse(csv);

        RejectedPerson rejection = result.Rejections.Single();

        Assert.AreEqual(2, rejection.LineNumber);
        Assert.AreEqual(invalidRow, rejection.Subject);
        Assert.AreEqual("EmailAddress is invalid.", rejection.Reason);
    }

    [TestMethod]
    public void ParseIncorrectColumnCountRejectsRecord()
    {
        string csv = $"{Header}\nJane,Doe,jane@example.com";

        PeopleCsvParseResult result = PeopleCsvParser.Parse(csv);

        Assert.IsEmpty(result.People);
        Assert.AreEqual(
            "Expected 4 columns but found 3.",
            result.Rejections.Single().Reason);
    }

    [TestMethod]
    public void ParseMalformedQuotedFieldRejectsRecord()
    {
        string csv =
            $"{Header}\nJane,\"Doe,jane@example.com,99201";

        PeopleCsvParseResult result = PeopleCsvParser.Parse(csv);

        Assert.IsEmpty(result.People);
        Assert.AreEqual(
            "A quoted CSV field was not closed.",
            result.Rejections.Single().Reason);
    }

    [TestMethod]
    public void ParseInvalidHeaderThrowsInvalidDataException()
    {
        string csv =
            "FirstName,LastName,Email,ZipCode\n" +
            "Jane,Doe,jane@example.com,99201";

        Assert.ThrowsExactly<InvalidDataException>(
            () => PeopleCsvParser.Parse(csv));
    }

    [TestMethod]
    public void ParseEmptyCsvThrowsInvalidDataException()
    {
        Assert.ThrowsExactly<InvalidDataException>(
            () => PeopleCsvParser.Parse(string.Empty));
    }

    [TestMethod]
    public void ParseHeaderOnlyReturnsEmptyResult()
    {
        PeopleCsvParseResult result = PeopleCsvParser.Parse(Header);

        Assert.IsEmpty(result.People);
        Assert.IsEmpty(result.Rejections);
    }

    [TestMethod]
    public void ParseUtf8BomHeaderSucceeds()
    {
        string csv =
            $"\uFEFF{Header}\nJane,Doe,jane@example.com,99201";

        PeopleCsvParseResult result = PeopleCsvParser.Parse(csv);

        Assert.AreEqual("Jane", result.People.Single().FirstName);
    }
}