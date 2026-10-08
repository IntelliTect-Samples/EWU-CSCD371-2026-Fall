using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void CreateLogger_WithoutConfiguration_ReturnsNull()
    {
        // Arrange
        var factory = new LogFactory();

        // Act
        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNull(logger);
    }

    [TestMethod]
    public void CreateLogger_WithConfiguration_ReturnsFileLogger()
    {
        // Arrange
        string path = Path.GetTempFileName();
        var factory = new LogFactory();
        factory.ConfigureFileLogger(path);

        // Act
        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNotNull(logger);
        Assert.IsInstanceOfType<FileLogger>(logger);
        Assert.AreEqual(nameof(LogFactoryTests), logger.ClassName);

        File.Delete(path);
    }

    [TestMethod]
    public void CreateLogger_UsesConfiguredFilePath()
    {
        // Arrange
        string path = Path.GetTempFileName();
        var factory = new LogFactory();
        factory.ConfigureFileLogger(path);

        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Act
        logger!.Log(LogLevel.Information, "Factory test message");

        // Assert
        string result = File.ReadAllText(path);

        StringAssert.Contains(result, "Factory test message");

        File.Delete(path);
    }

    [TestMethod]
    public void ConfigureFileLogger_CanChangeFilePath()
    {
        // Arrange
        string firstPath = Path.GetTempFileName();
        string secondPath = Path.GetTempFileName();

        var factory = new LogFactory();
        factory.ConfigureFileLogger(firstPath);
        factory.ConfigureFileLogger(secondPath);

        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Act
        logger!.Log(LogLevel.Debug, "Second path test");

        // Assert
        string firstResult = File.ReadAllText(firstPath);
        string secondResult = File.ReadAllText(secondPath);

        Assert.AreEqual(string.Empty, firstResult);
        StringAssert.Contains(secondResult, "Second path test");

        File.Delete(firstPath);
        File.Delete(secondPath);
    }
}
