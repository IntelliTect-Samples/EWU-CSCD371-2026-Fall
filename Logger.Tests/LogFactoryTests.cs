using System;
using System.Globalization;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void CreateLogger_WithoutConfiguration_ReturnsNull()
    {
        var factory = new LogFactory();
        var logger = factory.CreateLogger(nameof(LogFactoryTests));

        Assert.IsNull(logger);
    }

    [TestMethod]
    public void CreateLogger_WhenConfigured_ReturnsFileLoggerWithClassName()
    {
        var factory = new LogFactory();
        factory.ConfigureFileLogger("test-log.txt");
        var logger = factory.CreateLogger(nameof(LogFactoryTests));

        Assert.IsInstanceOfType<FileLogger>(logger);
        Assert.AreEqual(nameof(LogFactoryTests), logger.ClassName);
    }

    [TestMethod]
    [DataRow(LogLevel.Debug, "Test Message")]
    [DataRow(LogLevel.Error, "Test Message")]
    [DataRow(LogLevel.Information, "Test Message")]
    [DataRow(LogLevel.Warning, "Test Message")]
    public void CreateLogger_WhenConfigured_WritesEntryToConfiguredFile(LogLevel level, string message)
    {
        var factory = new LogFactory();
        string filePath = Path.GetTempFileName();
        try
        {
            factory.ConfigureFileLogger(filePath);
            var logger = factory.CreateLogger(nameof(LogFactoryTests));

            Assert.IsNotNull(logger);

            DateTime before = DateTime.Now;
            logger.Log(level, message);
            DateTime after = DateTime.Now;

            string[] lines = File.ReadAllLines(filePath);
            Assert.HasCount(1, lines);
            string contents = lines[0];

            Assert.Contains(message, contents);
            Assert.Contains(level.ToString(), contents);
            Assert.Contains(nameof(LogFactoryTests), contents);
            int classNamePosition = contents.IndexOf(nameof(LogFactoryTests), StringComparison.Ordinal);
            Assert.AreEqual($"{nameof(LogFactoryTests)} {level} : {message}", contents[classNamePosition..]);
            string timestampText = contents[..classNamePosition].Trim();
            Assert.IsTrue(DateTime.TryParseExact(
                timestampText,
                "G",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out DateTime timestamp));
            Assert.IsTrue(timestamp >= before.AddSeconds(-1) && timestamp <= after);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [TestMethod]
    public void ConfigureFileLogger_WhenReconfigured_PreservesExistingLoggerPath()
    {
        var factory = new LogFactory();
        string firstPath = Path.GetTempFileName();
        string secondPath = Path.GetTempFileName();
        try
        {
            factory.ConfigureFileLogger(firstPath);
            var firstLogger = factory.CreateLogger(nameof(LogFactoryTests));
            factory.ConfigureFileLogger(secondPath);
            var secondLogger = factory.CreateLogger(nameof(LogFactoryTests));

            Assert.IsNotNull(firstLogger);
            Assert.IsNotNull(secondLogger);

            firstLogger.Log(LogLevel.Warning, "First Message");
            secondLogger.Log(LogLevel.Warning, "Second Message");
            string[] firstLines = File.ReadAllLines(firstPath);
            string[] secondLines = File.ReadAllLines(secondPath);

            Assert.HasCount(1, firstLines);
            Assert.HasCount(1, secondLines);
            Assert.Contains("First Message", firstLines[0]);
            Assert.Contains("Second Message", secondLines[0]);
        }
        finally
        {
            File.Delete(firstPath);
            File.Delete(secondPath);
        }
    }

    [TestMethod]
    public void CreateLogger_WithNullClassName_ThrowsArgumentNullException()
    {
        var factory = new LogFactory();

        Assert.ThrowsExactly<ArgumentNullException>(() => factory.CreateLogger(null!));
    }

    [TestMethod]
    public void ConfigureFileLogger_WithNullPath_ThrowsArgumentNullException()
    {
        var factory = new LogFactory();

        Assert.ThrowsExactly<ArgumentNullException>(() => factory.ConfigureFileLogger(null!));
    }
}
