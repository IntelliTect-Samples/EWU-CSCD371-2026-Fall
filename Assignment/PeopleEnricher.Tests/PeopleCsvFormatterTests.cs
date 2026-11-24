namespace PeopleEnricher.Tests;

[TestClass]
public sealed class PeopleCsvFormatterTests
{
    private const string Header =
        "FirstName,LastName,EmailAddress,ZipCode,City,State";

    private static readonly EnrichedPerson Ada = new(
        "Ada",
        "Lovelace",
        "ada@example.com",
        "99001",
        "Spokane",
        "WA");

    private static readonly EnrichedPerson Alan = new(
        "Alan",
        "Turing",
        "alan@example.com",
        "99201",
        "Airway Heights",
        "WA");

    private static readonly PeopleCsvFormatter Formatter = new();

    [TestMethod]
    public void FormatWritesHeaderAndOneRowForOnePerson()
    {
        string csv = Formatter.Format([Ada]);

        Assert.AreEqual(
            $"{Header}\nAda,Lovelace,ada@example.com,99001,Spokane,WA\n",
            csv);
    }

    [TestMethod]
    public void FormatWritesOnlyTheHeaderWhenThereAreNoPeople()
    {
        string csv = Formatter.Format([]);

        Assert.AreEqual($"{Header}\n", csv);
    }

    [TestMethod]
    public void FormatWritesRowsInInputOrder()
    {
        string csv = Formatter.Format([Alan, Ada]);

        Assert.AreEqual(
            $"{Header}\n" +
            "Alan,Turing,alan@example.com,99201,Airway Heights,WA\n" +
            "Ada,Lovelace,ada@example.com,99001,Spokane,WA\n",
            csv);
    }

    [TestMethod]
    public void FormatWritesColumnsInTheRequiredOrder()
    {
        EnrichedPerson person = new("F", "L", "e@x.com", "11111", "C", "ST");

        string row = Formatter.Format([person]).Split('\n')[1];

        Assert.AreEqual("F,L,e@x.com,11111,C,ST", row);
    }

    [TestMethod]
    [DataRow("Spokane", "Spokane")]
    [DataRow("Salt Lake City", "Salt Lake City")]
    [DataRow("Washington, D.C.", "\"Washington, D.C.\"")]
    [DataRow("Say \"hi\"", "\"Say \"\"hi\"\"\"")]
    [DataRow("Line1\nLine2", "\"Line1\nLine2\"")]
    [DataRow("Line1\rLine2", "\"Line1\rLine2\"")]
    [DataRow("a,\"b\"", "\"a,\"\"b\"\"\"")]
    public void FormatQuotesAFieldOnlyWhenItNeedsQuoting(
        string city,
        string expectedCell)
    {
        EnrichedPerson person = Ada with { City = city };

        string csv = Formatter.Format([person]);

        Assert.AreEqual(
            $"{Header}\nAda,Lovelace,ada@example.com,99001,{expectedCell},WA\n",
            csv);
    }

    [TestMethod]
    public void FormatEscapesEveryColumn()
    {
        EnrichedPerson person = new(
            "A,da",
            "O\"Brien",
            "a@example.com",
            "99001",
            "Winston-Salem, NC",
            "NC");

        string csv = Formatter.Format([person]);

        Assert.AreEqual(
            $"{Header}\n\"A,da\",\"O\"\"Brien\",a@example.com,99001,\"Winston-Salem, NC\",NC\n",
            csv);
    }

    [TestMethod]
    public void FormatEnumeratesThePeopleExactlyOnce()
    {
        int enumerations = 0;

        IEnumerable<EnrichedPerson> Source()
        {
            enumerations++;
            yield return Ada;
            yield return Alan;
        }

        string csv = Formatter.Format(Source());

        Assert.AreEqual(1, enumerations);
        Assert.HasCount(3, csv.Split('\n', StringSplitOptions.RemoveEmptyEntries));
    }

    [TestMethod]
    public void FormatThrowsWhenPeopleIsNull() =>
        Assert.ThrowsExactly<ArgumentNullException>(
            () => Formatter.Format(null!));
}