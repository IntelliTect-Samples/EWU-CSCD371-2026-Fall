namespace Resilience.Tests;

[TestClass]
public class InvalidPersonRecordExceptionTester
{
    [TestMethod]
    public void Constructor_SavesProperties()
    {
        FormatException inner =
            new("Bad data");

        InvalidPersonRecordException ex =
            new(
                "Invalid record",
                3,
                "BAD,LINE",
                inner);

        Assert.AreEqual(
            3,
            ex.LineNumber);

        Assert.AreEqual(
            "BAD,LINE",
            ex.RawLine);

        Assert.AreSame(
            inner,
            ex.InnerException);
    }
}
