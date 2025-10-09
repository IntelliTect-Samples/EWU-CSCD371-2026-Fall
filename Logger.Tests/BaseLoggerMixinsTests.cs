using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Logger.Tests;

[TestClass]
public class BaseLoggerMixinsTests
{
    [TestMethod]
    //[ExpectedException(typeof(ArgumentNullException))]
    public void Error_WithNullLogger_ThrowsException()
    {
        Assert.ThrowsExactly<ArgumentNullException>(() =>
        BaseLoggerMixins.Error(null!, ""));
    }

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        var logger = new TestLogger();

        // Act
        logger.Error("Message {0}", 42);

        // Assert
        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);
        Assert.AreEqual("Message 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Warning_UsesCorrectLogLevel_Success()
    {
        TestLogger logger = new();

        logger.Warning("Low disk: {0}%", 12);

        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Warning, logger.LoggedMessages[0].LogLevel);
    }

    [TestMethod]
    public void Warning_FormatsMessageCorrectly_Success()
    {
        TestLogger logger = new();

        logger.Warning("Low disk: {0}%", 12);

        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual("Low disk: 12%", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Error_UsesCorrectLogLevel_Success()
    {
        TestLogger logger = new();

        logger.Error("System failure: {0}", "Disk error");

        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Error, logger.LoggedMessages[0].LogLevel);

    }

    [TestMethod]
    public void Error_FormatsMessageCorrectly_Success()
    { 
        TestLogger logger = new();

        logger.Error("System failure: {0}", "Disk error");

        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual("System failure: Disk error", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Debug_UsesCorrectLogLevel_Succes()
    {
        TestLogger logger = new();

        logger.Debug("Variable x = {0}", 42);

        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Debug, logger.LoggedMessages[0].LogLevel);
    }

    [TestMethod]
    public void Debug_FormatsMessageCorrectly_Success()
    {
        TestLogger logger = new();

        logger.Debug("Variable x = {0}", 42);

        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual("Variable x = 42", logger.LoggedMessages[0].Message);
    }

    [TestMethod]
    public void Information_UsesCorrectLogLevel_Succes()
    {
        TestLogger logger = new();

        logger.Information("User {0} logged in", "A");

        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual(LogLevel.Information, logger.LoggedMessages[0].LogLevel);

    }

    [TestMethod]
    public void Information_FormatsMessageCorrectly_Success()
    {
        TestLogger logger = new();

        logger.Information("User {0} logged in", "A");

        Assert.AreEqual(1, logger.LoggedMessages.Count);
        Assert.AreEqual("User A logged in", logger.LoggedMessages[0].Message);
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


