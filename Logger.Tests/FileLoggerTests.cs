using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Globalization;
using System.IO;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    [TestMethod]
    public void Log_WritesRequiredInformation()
    {
        // Arrange
        string path = Path.GetTempFileName();

        var logger = new FileLogger(path)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Act
        logger.Log(LogLevel.Warning, "Test message");

        // Assert
        string result = File.ReadAllText(path);

        StringAssert.Contains(
            result,
            DateTime.Now.Year.ToString(CultureInfo.InvariantCulture));
        StringAssert.Contains(result, nameof(FileLoggerTests));
        StringAssert.Contains(result, "Warning");
        StringAssert.Contains(result, "Test message");

        File.Delete(path);
    }

    [TestMethod]
    public void Log_AppendsMultipleMessages()
    {
        // Arrange
        string path = Path.GetTempFileName();

        var logger = new FileLogger(path)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Act
        logger.Log(LogLevel.Information, "First message");
        logger.Log(LogLevel.Error, "Second message");

        // Assert
        string[] lines = File.ReadAllLines(path);

        Assert.HasCount(2, lines);
        StringAssert.Contains(lines[0], "First message");
        StringAssert.Contains(lines[1], "Second message");

        File.Delete(path);
    }

    [TestMethod]
    public void Log_WritesClassName()
    {
        // Arrange
        string path = Path.GetTempFileName();

        var logger = new FileLogger(path)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Act
        logger.Log(LogLevel.Error, "Class name test");

        // Assert
        string result = File.ReadAllText(path);

        StringAssert.Contains(result, nameof(FileLoggerTests));

        File.Delete(path);
    }

    [TestMethod]
    public void Log_WritesCorrectLogLevel()
    {
        // Arrange
        string path = Path.GetTempFileName();

        var logger = new FileLogger(path)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Act
        logger.Log(LogLevel.Debug, "Debug test");

        // Assert
        string result = File.ReadAllText(path);

        StringAssert.Contains(result, "Debug");

        File.Delete(path);
    }
}