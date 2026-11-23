using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment9.Tests;

[TestClass]
public sealed class LocationCsvTests
{
    [TestMethod]
    [DataRow("\"SPOKANE\",\"WA\"", "SPOKANE", "WA")]
    [DataRow("\"Spokane\",\"WA\"\n", "Spokane", "WA")]
    [DataRow("\"COEUR D'ALENE\",\"ID\"", "COEUR D'ALENE", "ID")]
    [DataRow("\"ST. LOUIS\",\"MO\"", "ST. LOUIS", "MO")]
    [DataRow("\"WINSTON-SALEM\",\"NC\"", "WINSTON-SALEM", "NC")]
    public void Parse_ValidBody_ReturnsLocation(string body, string city, string state)
    {
        Assert.AreEqual(new Location(city, state), LocationCsv.Parse(body));
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("   ")]
    [DataRow("BAD_DATA")]
    [DataRow("\"SPOKANE\"")]
    [DataRow("\"SPOKANE\",\"WA\",\"99201\"")]
    [DataRow("\"\",\"WA\"")]
    [DataRow("\"SPOKANE\",\"\"")]
    [DataRow("\"SPOKANE\",\"XX\"")]
    [DataRow("\"SPOKANE\",\"wa\"")]
    [DataRow("\"SPOKANE\",\"WASHINGTON\"")]
    [DataRow("\"12345\",\"WA\"")]
    [DataRow("\"SPOKANE\",\"WA\"\n\"SEATTLE\",\"WA\"")]
    [DataRow("\"SPOKANE,\"WA\"")]
    public void Parse_InvalidBody_Throws(string body)
    {
        Assert.ThrowsExactly<InvalidResponseException>(() => LocationCsv.Parse(body));
    }
}
