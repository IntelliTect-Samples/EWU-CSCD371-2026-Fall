using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    private string _testFilePath = string.Empty;

    [TestInitialize]
    public void Setup()
    {
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_log_{Guid.NewGuid()}.txt");
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [TestMethod]
    public void LogFactory_CreateLogger_ReturnsNullWhenNotConfigured()
    {
        // Arrange
        var factory = new LogFactory();

        // Act
        var logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNull(logger);
    }

    [TestMethod]
    public void LogFactory_ConfigureFileLogger_StoresFilePath()
    {
        // Arrange
        var factory = new LogFactory();
        var filePath = _testFilePath;

        // Act
        factory.ConfigureFileLogger(filePath);
        var logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNotNull(logger);
        Assert.IsInstanceOfType<FileLogger>(logger);
    }

    [TestMethod]
    public void LogFactory_CreateLogger_ReturnsFileLoggerAfterConfiguration()
    {
        // Arrange
        var factory = new LogFactory();
        factory.ConfigureFileLogger(_testFilePath);

        // Act
        var logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNotNull(logger);
        Assert.IsInstanceOfType<BaseLogger>(logger);
    }

    [TestMethod]
    public void LogFactory_CreateLogger_SetsClassNameUsingObjectInitializer()
    {
        // Arrange
        var factory = new LogFactory();
        factory.ConfigureFileLogger(_testFilePath);
        var expectedClassName = nameof(LogFactoryTests);

        // Act
        var logger = factory.CreateLogger(expectedClassName);

        // Assert
        Assert.IsNotNull(logger);
        Assert.AreEqual(expectedClassName, logger!.ClassName);
    }

    [TestMethod]
    public void LogFactory_CreateLogger_LogsWithCorrectClassName()
    {
        // Arrange
        var factory = new LogFactory();
        factory.ConfigureFileLogger(_testFilePath);
        var className = nameof(LogFactoryTests);

        // Act
        var logger = factory.CreateLogger(className);
        logger!.Log(LogLevel.Information, "Test message");

        // Assert
        var content = File.ReadAllText(_testFilePath);
        Assert.Contains(className, content);
    }

    [TestMethod]
    public void LogFactory_CreateLogger_MultipleCallsCreateSeparateInstances()
    {
        // Arrange
        var factory = new LogFactory();
        factory.ConfigureFileLogger(_testFilePath);

        // Act
        var logger1 = factory.CreateLogger(nameof(LogFactoryTests));
        var logger2 = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNotNull(logger1);
        Assert.IsNotNull(logger2);
        Assert.AreNotSame(logger1, logger2);
    }

    [TestMethod]
    public void LogFactory_ConfigureFileLogger_CanBeCalledMultipleTimes()
    {
        // Arrange
        var factory = new LogFactory();
        var filePath1 = _testFilePath;
        var filePath2 = Path.Combine(Path.GetTempPath(), $"test_log_{Guid.NewGuid()}.txt");

        // Act
        factory.ConfigureFileLogger(filePath1);
        var logger1 = factory.CreateLogger(nameof(LogFactoryTests));

        factory.ConfigureFileLogger(filePath2);
        var logger2 = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNotNull(logger1);
        Assert.IsNotNull(logger2);

        // Cleanup second file
        if (File.Exists(filePath2))
        {
            File.Delete(filePath2);
        }
    }
}
