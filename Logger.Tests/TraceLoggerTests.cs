using System.Diagnostics;
using System.Globalization;


namespace Logger.Tests;

[TestClass]
public class TraceLoggerTests
{
    /// <summary>
    /// Used to test TraceLogger.Create
    /// Tests correctness of ClassName
    /// </summary>
    [TestMethod]
    public void TestCreateTraceLogger()
    {
        TraceLogger logger = TraceLogger.CreateLogger(nameof(TraceLoggerTests));
        Assert.EndsWith(nameof(TraceLoggerTests), logger.ClassName);
    }

    /// <summary>
    /// Used to test TraceLogger.Log output
    /// </summary>
    [TestMethod]
    public void TestTraceLoggerOutput()
    {
        string path = Path.GetTempFileName();
        TraceListener listener = new TextWriterTraceListener(path);

        Trace.Listeners.Add(listener); // Needed to read the output of Trace.Writeline
        Trace.AutoFlush = true;

        try
        {
            TraceLogger logger = TraceLogger.CreateLogger(nameof(TraceLoggerTests));

            logger.Log(LogLevel.Warning, "First");
            logger.Log(LogLevel.Error, "Second");
        }
        finally
        {
            Trace.Listeners.Remove(listener);
            listener.Dispose();
        }
        
        string[] lines = File.ReadAllLines(path);
        Assert.HasCount(2, lines);

        try
        {
            Assert.Contains(nameof(TraceLoggerTests), lines[0]);
            Assert.Contains("Warning", lines[0]);
            Assert.Contains("First", lines[0]);
            Assert.Contains(DateTime.Now.Year.ToString(CultureInfo.CurrentCulture), lines[0]);
            Assert.Contains("Error: Second", lines[1]);
        }
        finally
        {
            File.Delete(path);
        }

    }
}
