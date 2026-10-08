using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void CreateLogger_WhenNotConfigured_ReturnsNull()
    {
        // Arrange
        var factory = new LogFactory();

        // Act
        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNull(logger);
    }

    [TestMethod]
    public void CreateLogger_WhenConfigured_ReturnsFileLoggerWithClassName()
    {
        // Arrange
        var factory = new LogFactory();
        factory.ConfigureFileLogger("test.log");

        // Act
        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNotNull(logger);
        Assert.IsInstanceOfType<FileLogger>(logger);
        Assert.AreEqual(nameof(LogFactoryTests), logger.ClassName);
    }

    [TestMethod]
    public void CreateLogger_WhenConfigured_UsesConfiguredFilePath()
    {
        string filePath = Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid():N}.log");

        try
        {
            var factory = new LogFactory();
            factory.ConfigureFileLogger(filePath);

            BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

            Assert.IsNotNull(logger);

            logger.Log(LogLevel.Information, "Test message");

            Assert.IsTrue(File.Exists(filePath));

            string contents = File.ReadAllText(filePath);

            Assert.IsTrue(contents.Contains("Test message"));
            Assert.IsTrue(contents.Contains(nameof(LogFactoryTests)));
            Assert.IsTrue(contents.Contains("Information"));
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }
}