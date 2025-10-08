using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Logger.Tests;

[TestClass]
public class BaseLoggerMixinsTests
{
    [TestMethod]
    public void Error_WithNullLogger_ThrowsException()
    {
        // Arrange
        TestLogger logger = new();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Error(null, ""));
    }

    [TestMethod]
    public void Warning_NullLogger_ThrowsArgumentNullException()
    {
        // Arrange
        TestLogger logger = new();

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Warning(null, ""));
    }

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new();

        // Act
        logger.Error("Message {0}", 42);

        // Assert
        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }


    [TestMethod]
    public void Information_ValidLogger_FormatsMessageCorrectly()
    {
        // Arrange
        TestLogger logger = new();

        // Act
        logger.Information("User {0} successfully logged in", "Bob");

        // Assert
        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("User Bob successfully logged in", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Debug_ValidLogger_RecordsMessageWithDebugLevel()
    {
        // Arrange
        TestLogger logger = new();

        // Act
        logger.Debug("Debugging process step");

        // Assert
        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Debugging process step", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_ValidLogger_LogsWarningMessage()
    {
        // Arrange
        TestLogger logger = new();

        // Act
        logger.Warning("Disk space is running low");

        // Assert
        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Disk space is running low", logger.LoggedMessages[0].Message);
    }

}

public class TestLogger : BaseLogger
{
    public List<(LogLevel LogLevel, string Message)> LoggedMessages { get; } = new List<(LogLevel, string)>();

    public override void Log(LogLevel logLevel, string message)
    {
        LoggedMessages.Add((logLevel, message));
    }
}
