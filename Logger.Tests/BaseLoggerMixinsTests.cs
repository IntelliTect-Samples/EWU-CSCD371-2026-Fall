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

        //Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Error(logger, "This should throw null exception"));
    }

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Error("Message {0}", 42);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? logger = null;

        //Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Warning(logger, "This should throw null exception"));
    }

    [TestMethod]
    public void Information_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? logger = null;

        //Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Information(logger, "This should throw null exception"));
    }
    
    [TestMethod]
    public void Debug_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? logger = null;

        //Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Debug(logger, "This should throw null exception"));
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
