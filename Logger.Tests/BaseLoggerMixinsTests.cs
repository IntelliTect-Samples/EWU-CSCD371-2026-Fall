using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Logger.Tests;

[TestClass]
public class BaseLoggerMixinsTests
{
    #region Extention Methods : Exception
    [TestMethod]
    public void LogType_WithNullLogger_ThrowsException()
    {
        // Arrange
        FileLogger fileLogger = null!;
        // Act
        // Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Error("Message"));
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Debug("Message"));
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Warning("Message"));
        Assert.ThrowsExactly<ArgumentNullException>(() => fileLogger.Information("Message"));
    }
    #endregion
    #region Extention Methods : Valid
    [TestMethod]
    public void AllLevels_WithNoData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Error("Message");
        logger.Warning("Message");
        logger.Information("Message");
        logger.Debug("Message");

        // Assert
        Assert.AreEqual(4, logger.LoggedMessages.Count);

        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message", logger.LoggedMessages[0].Message);

        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[1].LogLevel);
        Assert.AreEqual("Message", logger.LoggedMessages[1].Message);

        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[2].LogLevel);
        Assert.AreEqual("Message", logger.LoggedMessages[2].Message);

        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[3].LogLevel);
        Assert.AreEqual("Message", logger.LoggedMessages[3].Message);

    }
    #endregion
    #region With Params : Valid
    [TestMethod]
    public void AllLevels_WithData_LogsMessage()
    {
        // Arrange
        TestLogger logger = new TestLogger();

        // Act
        logger.Error("Message {0}", 1);
        logger.Warning("Message {0}", 2);
        logger.Information("Message {0}", 3);
        logger.Debug("Message {0}", 4);
        //multiple methods Test
        logger.Information("This Is a Multi param test {0} {0} {1} {2}", 4, "Who am i?", "Just another object?");

        // Assert
        Assert.AreEqual(5, logger.LoggedMessages.Count);

        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 1", logger.LoggedMessages[0].Message);

        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[1].LogLevel);
        Assert.AreEqual("Message 2", logger.LoggedMessages[1].Message);

        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[2].LogLevel);
        Assert.AreEqual("Message 3", logger.LoggedMessages[2].Message);

        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[3].LogLevel);
        Assert.AreEqual("Message 4", logger.LoggedMessages[3].Message);

        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[4].LogLevel);
        Assert.AreEqual("This Is a Multi param test 4 4 Who am i? Just another object?", logger.LoggedMessages[4].Message);
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
