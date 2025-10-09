using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;

using Logger;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class BaseLoggerMixinsTests
{
    [TestMethod]
    public void Error_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? nullLogger = null;
        string message = "Test Error message.";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Error(nullLogger!, message));
 
    }

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger();
        string format = "Test: Critical failure in module {0}.";
        object[] args = { "TestModule" };
        string expectedMessage = "Test: Critical failure in module TestModule.";

        // Act
        logger.Error(format, args);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual(expectedMessage, logger.LoggedMessages[0].Message);
    }

    // Test Warnings
    [TestMethod]
    public void Warning_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? nullLogger = null;
        string message = "Test Warning message.";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Warning(nullLogger!, message));
    }

    [TestMethod]
    public void Warning_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger();
        string format = "Test: Config value {0} is outside recommended range.";
        object[] args = { "TestThreads" };
        string expectedMessage = "Test: Config value TestThreads is outside recommended range.";

        // Act
        logger.Warning(format, args);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual(expectedMessage, logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Information_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? nullLogger = null;
        string message = "Test Information message.";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Information(nullLogger!, message));
        
    }

    [TestMethod]
    public void Information_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger();
        string format = "Test: Application startup in {0}ms.";
        object[] args = { 2300 };
        string expectedMessage = "Test: Application startup in 2300ms.";

        // Act
        logger.Information(format, args);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual(expectedMessage, logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Debug_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger? nullLogger = null;
        string message = "Test Debug message.";

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => BaseLoggerMixins.Debug(nullLogger!, message));
        
    }

    [TestMethod]
    public void Debug_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger();
        string format = "Test: Processed array length: {0}. Checksum: {1:X}.";
        object[] args = { 22, 0xDEADBEEF };
        string expectedMessage = "Test: Processed array length: 22. Checksum: DEADBEEF.";

        // Act
        logger.Debug(format, args);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual(expectedMessage, logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void BaseLoggerMixins_Uses_InvariantCulture_For_Formatting()
    {
        // Arrange
        var logger = new TestLogger();
        string expectedMessage = "Value: 1234.57";
        var originalCulture = Thread.CurrentThread.CurrentCulture;
        Thread.CurrentThread.CurrentCulture = new CultureInfo("fr-FR");

        try
        {
            // Act
            logger.Information("Value: {0}", 1234.57);

            // Assert
            Assert.AreEqual(1, logger.LoggedMessages.Count);
            Assert.AreEqual(expectedMessage, logger.LoggedMessages[0].Message);

        } finally
        {
            Thread.CurrentThread.CurrentCulture = originalCulture;
        }
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

