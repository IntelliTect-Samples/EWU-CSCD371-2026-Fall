using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void CreateLogger_SetsLoggerClassName_Success()
    {
        // Arrange
        LogFactory logFactory = new();
        string className = "TestClass";
        string filePath = "Testpath";
        logFactory.ConfigureFileLogger(filePath);

        // Act
        BaseLogger? logger = logFactory.CreateLogger(className);

        // Assert
        Assert.IsNotNull(logger);
        Assert.AreEqual(className, logger.LoggerClassName);
    }

    [TestMethod]
    public void CreateLogger_NotConfigured_ReturnsNull()
    {
        // Arrange
        LogFactory logFactory = new();
        string className = "TestClass";
        // no configuring

        // Act
        BaseLogger? logger = logFactory.CreateLogger(className);

        // Assert
        Assert.IsNull(logger);
    }
}
