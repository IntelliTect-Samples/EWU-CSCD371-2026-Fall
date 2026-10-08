using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Collections.Generic;

namespace Logger.Tests;

[TestClass]
public class BaseLoggerExtensionsTests
{
    [TestMethod]
    public void Error_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger logger = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Error(""));
    }

    [TestMethod]
    public void Warning_WithNullLogger_ThrowsException()
    {
        BaseLogger logger = null!;

        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Warning(""));
    }

    [TestMethod]
    public void Information_WithNullLogger_ThrowsException()
    {
        BaseLogger logger = null!;

        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Information(""));
    }

    [TestMethod]
    public void Debug_WithNullLogger_ThrowsException()
    {
        BaseLogger logger = null!;

        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Debug(""));
    }

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger{ ClassName = nameof(BaseLoggerExtensionsTests) };

        // Act
        logger.Error("Message {0}", 42);

        // Assert
        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_WithData_LogsMessage()
    {
        var logger = new TestLogger{ ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Warning("Disk at {0}% after {1} writes", 90, 3);

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Disk at 90% after 3 writes", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Information_WithData_LogsMessage()
    {
        var logger = new TestLogger{ ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Information("User {0} signed in", "inigo");

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("User inigo signed in", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Debug_WithNoArgs_LogsMessageUnchanged()
    {
        var logger = new TestLogger{ ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Debug("Plain message");

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Plain message", logger.LoggedMessages[0].Message);
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
