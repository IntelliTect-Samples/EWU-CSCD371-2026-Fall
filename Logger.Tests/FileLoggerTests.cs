using System.Globalization;
namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    /// <summary>
    /// Used for testing the log file output
    /// </summary>
    [TestMethod]
    public void LogAppendsMessagesOnSeparateLines()
    {
        string path = Path.GetTempFileName();
        try
        {
            var logger = new FileLogger(path) { ClassName = nameof(FileLoggerTests) };
            //Resolves new years error 
            int currYear = DateTime.Now.Year;

            logger.Log(LogLevel.Warning, "First");
            logger.Log(LogLevel.Error, "Second");

            string[] lines = File.ReadAllLines(path);
            Assert.HasCount(2, lines);
            Assert.Contains(nameof(FileLoggerTests), lines[0]);
            Assert.Contains("Warning", lines[0]);
            Assert.Contains("First", lines[0]);
            Assert.Contains(currYear.ToString(CultureInfo.CurrentCulture), lines[0]);
            Assert.Contains("Error: Second", lines[1]);
        }
        finally
        {
            File.Delete(path);
        }
    }

    /// <summary>
    /// Used for testing constructor
    /// </summary>
    [TestMethod]
    public void ConstructorThrowsOnNullPath()
    {
        Assert.ThrowsExactly<ArgumentNullException>(
            () => new FileLogger(null!) { ClassName = nameof(FileLoggerTests) });
    }
}