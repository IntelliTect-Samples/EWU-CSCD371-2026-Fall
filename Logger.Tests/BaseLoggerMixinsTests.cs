using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Logger.Tests;

[TestClass]
public class BaseLoggerMixinsTests
{
    #region Extention Methods : Exception
    [TestMethod]
    public void Error_WithNullLogger_ThrowsException()
    {
        // Arrange
        FileLogger fileLogger = null!;
        // Act
        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Error("Message"));
    }

    [TestMethod]
    public void Debug_WithNullLogger_ThrowsException()
    {
        // Arrange
        FileLogger fileLogger = null!;
        // Act
        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Debug("Message"));
    }

    [TestMethod]
    public void Warning_WithNullLogger_ThrowsException()
    {
        // Arrange
        FileLogger fileLogger = null!;
        // Act
        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Warning("Message"));
    }

    [TestMethod]
    public void Information_WithNullLogger_ThrowsException()
    {
        // Arrange
        FileLogger fileLogger = null!;
        // Act
        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Information("Message"));
    }
    #endregion

    #region Extention Methods : Valid
    [TestMethod]
    public void Debug_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Debug("Message");

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);

        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Error("Message");

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);

        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Warning("Message");

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);

        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Information_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Information("Message");

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);

        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message", logger.LoggedMessages[0].Message);
    }
    #endregion
}

public class TestLogger : BaseLogger
{
    public List<(LogLevel LogLevel, string Message)> LoggedMessages { get; } = new List<(LogLevel, string)>();

    public override void Log(LogLevel logLevel, string message)
    {
        LoggedMessages.Add((logLevel, message));
    }

    public new static BaseLogger? Create(string className, string? filePath = null)
    {
        return new TestLogger
        {
            SourceClassName = className
        };
    }

}
