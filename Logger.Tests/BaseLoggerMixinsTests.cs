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

        // Act
        FileLogger fileLogger = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Error("Message"));
        // Assert
    }
    [TestMethod]
    public void Debug_WithNullLogger_ThrowsException()
    {
        // Arrange

        // Act
        FileLogger fileLogger = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Debug("Message"));
        // Assert
    }
    [TestMethod]
    public void Warning_WithNullLogger_ThrowsException()
    {
        // Arrange

        // Act
        FileLogger fileLogger = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Warning("Message"));
        // Assert
    }
    [TestMethod]
    public void Information_WithNullLogger_ThrowsException()
    {
        // Arrange

        // Act
        FileLogger fileLogger = null!;
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Information("Message"));
        // Assert
    }
    #endregion
    #region Extention Methods : Valid
    [TestMethod]
    public void Error_WithValidLogger_Success()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Error("test");
        // Assert
        Assert.AreEqual((LogLevel.Error, "test"), logger.LoggedMessages[0]);
    }
    [TestMethod]
    public void Debug_WithValidLogger_Success()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Debug("test");
        // Assert
        Assert.AreEqual((LogLevel.Debug, "test"), logger.LoggedMessages[0]);
    }
    [TestMethod]
    public void Warning_WithValidLogger_Success()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Warning("test");
        // Assert
        Assert.AreEqual((LogLevel.Warning, "test"), logger.LoggedMessages[0]);
    }
    [TestMethod]
    public void Information_WithValidLogger_Success()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Information("test");
        // Assert
        Assert.AreEqual((LogLevel.Information, "test"), logger.LoggedMessages[0]);
    }
    #endregion

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        //logger.Error("Message {0}", 42);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
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
