using Microsoft.VisualStudio.TestTools.UnitTesting;
using Assignment;

namespace Assignment.Tests;

[TestClass]
public class CsvParserTests
{
    // -------------------------------------------------------------------------
    // TESTS FOR: ParseStateFromCsvRow
    // -------------------------------------------------------------------------

    [TestMethod]
    public void ParseState_ValidRow_ReturnsCorrectState()
    {
        // Index 5 is the state
        // 0:First, 1:Last, 2:Email, 3:Street, 4:City, 5:State, 6:Zip
        var row = "John,Doe,email@test.com,123 Main,Seattle,WA,98101";

        var result = CsvParser.ParseStateFromCsvRow(row);

        Assert.AreEqual("WA", result);
    }

    [TestMethod]
    public void ParseState_RowWithWhitespace_TrimsState()
    {
        // Notice the spaces around " OR "
        var row = "Jane,Smith,email@test.com,456 Oak,Portland, OR ,97201";

        var result = CsvParser.ParseStateFromCsvRow(row);

        Assert.AreEqual("OR", result, "Parser should trim leading/trailing whitespace.");
    }

    [TestMethod]
    public void ParseState_ShortRow_ReturnsEmptyString()
    {
        // Logic check: parts.Length > 5
        // This row only has indices 0, 1, 2
        var row = "Bob,Jones,bob@example.com";

        var result = CsvParser.ParseStateFromCsvRow(row);

        Assert.AreEqual(string.Empty, result, "Should return empty string if index 5 does not exist.");
    }

    [TestMethod]
    public void ParseState_EmptyString_ReturnsEmptyString()
    {
        var result = CsvParser.ParseStateFromCsvRow("");
        Assert.AreEqual(string.Empty, result);
    }

    // -------------------------------------------------------------------------
    // TESTS FOR: ParsePersonFromCsvRow
    // -------------------------------------------------------------------------

    [TestMethod]
    public void ParsePerson_ValidRow_MapsAllProperties()
    {
        // 0:First, 1:Last, 2:Email, 3:Street, 4:City, 5:State, 6:Zip
        var row = "Alice,Wonder,alice@test.com,1 Rabbit Hole,Wonderland,WA,12345";

        var person = CsvParser.ParsePersonFromCsvRow(row);

        // Verify Person details
        Assert.IsNotNull(person);
        Assert.AreEqual("Alice", person.FirstName);
        Assert.AreEqual("Wonder", person.LastName);
        Assert.AreEqual("alice@test.com", person.EmailAddress);

        // Verify Address details
        Assert.IsNotNull(person.Address);
        Assert.AreEqual("1 Rabbit Hole", person.Address.StreetAddress);
        Assert.AreEqual("Wonderland", person.Address.City);
        Assert.AreEqual("WA", person.Address.State);
        Assert.AreEqual("12345", person.Address.Zip);
    }

    [TestMethod]
    public void ParsePerson_RowWithWhitespace_TrimsAllFields()
    {
        // Input has deliberate spaces
        var row = "  Bob  ,  Builder  ,  bob@build.com  ,  123 Brick Ln  ,  London  ,  UK  ,  55555  ";

        var person = CsvParser.ParsePersonFromCsvRow(row);

        Assert.AreEqual("Bob", person.FirstName);
        Assert.AreEqual("Builder", person.LastName);
        Assert.AreEqual("bob@build.com", person.EmailAddress);
        Assert.AreEqual("UK", person.Address.State);
    }

    [TestMethod]
    public void ParsePerson_MissingAddressInfo_DefaultsToEmptyStrings()
    {
        // Row cuts off after Email (Index 2)
        // The parser logic checks parts.Length > 3, > 4, etc.
        var row = "No,Home,nomad@test.com";

        var person = CsvParser.ParsePersonFromCsvRow(row);

        Assert.AreEqual("No", person.FirstName);
        Assert.AreEqual("nomad@test.com", person.EmailAddress);

        // Address should exist but contain empty strings
        Assert.IsNotNull(person.Address);
        Assert.AreEqual(string.Empty, person.Address.StreetAddress);
        Assert.AreEqual(string.Empty, person.Address.City);
        Assert.AreEqual(string.Empty, person.Address.State);
        Assert.AreEqual(string.Empty, person.Address.Zip);
    }

    [TestMethod]
    public void ParsePerson_EmptyRow_ReturnsEmptyObjects()
    {
        var person = CsvParser.ParsePersonFromCsvRow("");

        Assert.AreEqual(string.Empty, person.FirstName);
        Assert.AreEqual(string.Empty, person.LastName);
        Assert.IsNotNull(person.Address);
        Assert.AreEqual(string.Empty, person.Address.State);
    }
}