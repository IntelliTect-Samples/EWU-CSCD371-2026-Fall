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
        BaseLogger? logger = null;

        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Error(""));
    }

    [TestMethod]
    public void Error_WithOneArgument_LogsFormattedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Error("Message {0}", 42);

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_WithNullLogger_ThrowsException()
    {
        BaseLogger? logger = null;

        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Warning(""));
    }

    [TestMethod]
    public void Warning_WithOneArgument_LogsFormattedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Warning("Message {0}", 42);

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Information_WithNullLogger_ThrowsException()
    {
        BaseLogger? logger = null;

        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Information(""));
    }

    [TestMethod]
    public void Information_WithOneArgument_LogsFormattedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Information("Message {0}", 42);

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Debug_WithNullLogger_ThrowsException()
    {
        BaseLogger? logger = null;

        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Debug(""));
    }

    [TestMethod]
    public void Debug_WithOneArgument_LogsFormattedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Debug("Message {0}", 42);

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Error_WithoutArguments_LogsUnchangedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };
        logger.Error("Plain message");

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Plain message", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_WithoutArguments_LogsUnchangedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };
        logger.Warning("Plain message");

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Plain message", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Information_WithoutArguments_LogsUnchangedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };
        logger.Information("Plain message");

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Plain message", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Debug_WithoutArguments_LogsUnchangedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };
        logger.Debug("Plain message");

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Plain message", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Error_WithMultipleArguments_LogsFormattedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Error("You {0} {1}", "are", 42);

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("You are 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Debug_WithMultipleArguments_LogsFormattedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Debug("You {0} {1}", "are", 42);

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("You are 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_WithMultipleArguments_LogsFormattedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Warning("You {0} {1}", "are", 42);

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("You are 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Information_WithMultipleArguments_LogsFormattedMessage()
    {
        var logger = new TestLogger() { ClassName = nameof(BaseLoggerExtensionsTests) };

        logger.Information("You {0} {1}", "are", 42);

        Assert.HasCount(1, logger.LoggedMessages);
        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("You are 42", logger.LoggedMessages[0].Message);
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
