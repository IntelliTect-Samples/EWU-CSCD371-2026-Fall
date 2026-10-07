using System;
using System.Collections.Generic;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class BaseLoggerExtensionsTests
{
    [TestMethod]
    public void Error_WithNullLogger_ThrowsException()
    {
        // Arrange

        // Act
        Assert.Throws<ArgumentNullException>(() =>
        BaseLoggerExtensions.Error(null!, ""));

        // Assert
       
    }

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger();

        // Act
        logger.Error("Message {0}", 42);

        // Assert
        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Information_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger();

        // Act
        logger.Information("Message {0}", 42);

        // Assert
        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Debug_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger();

        // Act
        logger.Debug("Message {0}", 42);

        // Assert
        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger();

        // Act
        logger.Warning("Message {0}", 42);

        // Assert
        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_WithNullLogger_ThrowsException()
    {
        // Arrange

        // Act

        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            BaseLoggerExtensions.Warning(null!, ""));
    }

    [TestMethod]
    public void Information_WithNullLogger_ThrowsException()
    {
        // Arrange

        // Act

        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            BaseLoggerExtensions.Information(null!, ""));
    }

    [TestMethod]
    public void Debug_WithNullLogger_ThrowsException()
    {
        // Arrange

        // Act

        // Assert
        Assert.Throws<ArgumentNullException>(() =>
            BaseLoggerExtensions.Debug(null!, ""));
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
