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

    [TestMethod]
    public void Constructor_NoArgs_DefaultsRawLineToStringDotEmpty()
    {
        InvalidPersonRecordException ex = new();
        Assert.IsEmpty(ex.RawLine);
    }

    [TestMethod]
    public void Constructor_OneArg_PassesMessageAndDefaultRawLine()
    {
        string message = "Data bad";
        InvalidPersonRecordException ex = new(message);
        Assert.AreEqual(message, ex.Message);
        Assert.IsEmpty(ex.RawLine);
    }

    [TestMethod]
    public void Constructor_TwoArgs_PassesMessageAndInnerDefaultsRawLine()
    {
        string innerMessage = "Bad argument";
        string message = "Bad data";
        ArgumentException inner = new(innerMessage);
        InvalidPersonRecordException ex = new(message, inner);


        Assert.AreEqual(inner, ex.InnerException);
        Assert.IsEmpty(ex.RawLine);
    }
}
