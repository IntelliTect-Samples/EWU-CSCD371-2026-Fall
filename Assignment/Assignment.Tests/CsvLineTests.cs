using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Assignment9.Tests;

[TestClass]
public sealed class CsvLineTests
{
    private static readonly string[] expectedUnquoted = ["a", "b", "", "d"];
    private static readonly string[] expectedQuoted = ["Spokane, WA", "say \"hi\""];

    [TestMethod]
    public void TryParse_UnquotedFields_SplitsOnCommas()
    {
        Assert.IsTrue(CsvLine.TryParse("a,b,,d", out IReadOnlyList<string> fields));
        CollectionAssert.AreEqual(expectedUnquoted, fields.ToArray());
    }

    [TestMethod]
    public void TryParse_QuotedFields_RemovesQuotesAndUnescapesDoubledQuotes()
    {
        Assert.IsTrue(CsvLine.TryParse("\"Spokane, WA\",\"say \"\"hi\"\"\"", out IReadOnlyList<string> fields));
        CollectionAssert.AreEqual(expectedQuoted, fields.ToArray());
    }

    [TestMethod]
    [DataRow("\"unterminated")]
    [DataRow("ab\"c,d")]
    [DataRow("\"abc\"x,d")]
    public void TryParse_MalformedLine_ReturnsFalse(string line)
    {
        Assert.IsFalse(CsvLine.TryParse(line, out _));
    }

    [TestMethod]
    public void Format_QuotesOnlyFieldsThatNeedIt()
    {
        Assert.AreEqual("plain,\"a,b\",\"q\"\"t\"", CsvLine.Format("plain", "a,b", "q\"t"));
    }

    [TestMethod]
    public void Format_ThenTryParse_RoundTrips()
    {
        string[] original = ["x", "comma, here", "\"quoted\"", ""];
        Assert.IsTrue(CsvLine.TryParse(CsvLine.Format(original), out IReadOnlyList<string> parsed));
        CollectionAssert.AreEqual(original, parsed.ToArray());
    }
}
