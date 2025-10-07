using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Logger; // Add this to use Logger.BaseLogger and Logger.LogLevel

namespace Logger.Tests;

[TestClass]
public class BaseLoggerMixinsTests
{
    [TestMethod]
    public void Error_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? logger = null;
        string message = "";

        // Act
        void Act() => BaseLoggerMixins.Error(logger, message);

        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);
    }

    [TestMethod]
    public void EnsureLogger_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? logger = null;
        // Act
        void Act() => BaseLoggerMixins.EnsureLogger(logger);
        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(Act);
    }

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new();

        // Act
        logger.Error("Message {0}", 42);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Error_WithMultipleArguments_FormatsCorrectly()
    {
        // Arrange
        TestLogger logger = new();
        // Act
        logger.Error("Values: {0}, {1}", "A", "B");
        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Values: A, B", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new();

        // Act
        logger.Warning("Message {0}", 42);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Information_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new();

        // Act
        logger.Information("Message {0}", 42);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Debug_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new();

        // Act
        logger.Debug("Message {0}", 42);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

}

public class TestLogger : BaseLogger
{
    public List<(LogLevel LogLevel, string Message)> LoggedMessages { get; } = new List<(LogLevel, string)>();

    public string ClassName => throw new NotImplementedException();


    public void Log(LogLevel logLevel, string message)
    {
        LoggedMessages.Add((logLevel, message));
    }
}
// Remove the duplicate BaseLogger and LogLevel definitions
