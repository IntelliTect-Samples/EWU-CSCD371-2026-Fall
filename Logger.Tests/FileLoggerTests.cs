using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    [TestMethod]
    public void Log_WritesMessageToFile()
    {
        string path = Path.GetTempFileName();

        FileLogger logger = new FileLogger(path);
        logger.ClassName = nameof(FileLoggerTests);

        logger.Log(LogLevel.Warning, "Test message");

        string result = File.ReadAllText(path);

        StringAssert.Contains(result, "FileLoggerTests Warning: Test message");
    }

    [TestMethod]
    public void Log_AppendsMultipleMessages()
    {
        // Arrange
        string path = Path.GetTempFileName();

        FileLogger logger = new FileLogger(path);
        logger.ClassName = nameof(FileLoggerTests);

        // Act
        logger.Log(LogLevel.Warning, "First message");
        logger.Log(LogLevel.Error, "Second message");

        // Assert
        string result = File.ReadAllText(path);

        StringAssert.Contains(result, "First message");
        StringAssert.Contains(result, "Second message");
    }
}
