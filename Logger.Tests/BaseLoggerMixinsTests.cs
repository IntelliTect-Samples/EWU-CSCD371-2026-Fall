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
        BaseLogger? logger = null;

        // Act / Assert - this method 'shuts up the compiler'
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Error(logger!, ""));
    }

    [TestMethod]
    public void Warning_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? logger = null;

        // Act / Assert - this method 'shuts up the compiler'
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Warning(logger!, ""));
    }

    [TestMethod]
    public void Information_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? logger = null;

        // Act / Assert - this method 'shuts up the compiler'
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Information(logger!, ""));
    }

    [TestMethod]
    public void Debug_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? logger = null;

        // Act / Assert - this method 'shuts up the compiler'
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Debug(logger!, ""));
    }

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger { LoggerClassName = "TestLogger" };

        // Act
        logger.Error("Message {0}", 42);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger { LoggerClassName = "TestLogger" };

        // Act
        logger.Warning("Message {0}", 43);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 43", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Information_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger { LoggerClassName = "TestLogger" };

        // Act
        logger.Information("Message {0}", 44);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 44", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Debug_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger { LoggerClassName = "TestLogger" };

        // Act
        logger.Debug("Message {0}", 45);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 45", logger.LoggedMessages[0].Message);
    }
    
    [TestMethod]
    public void Information_WithNoArgs_PassesMessageUnchanged()
    {
        // Arrange
        var logger = new TestLogger { LoggerClassName = "TestLogger" };

        // Act
        logger.Information("Hello world");

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(logger.LoggedMessages[0].LogLevel, LogLevel.Information);
        Assert.AreEqual("Hello world", logger.LoggedMessages[0].Message);
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
