using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.IO;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void CreateLogger_NotConfigured_ReturnsNull()
    {
        var factory = new LogFactory();

        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        Assert.IsNull(logger);
    }

    [TestMethod]
    public void CreateLogger_Configured_ReturnsFileLoggerWithConfiguredPath()
    {
        // Arrange
        var factory = new LogFactory();
        string filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        factory.ConfigureFileLogger(filePath);

        // Act
        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        FileLogger fileLogger = Assert.IsInstanceOfType<FileLogger>(logger);
        Assert.AreEqual(filePath, fileLogger.FilePath);
    }

    [TestMethod]
    public void CreateLogger_Configured_SetsClassName()
    {
        var factory = new LogFactory();
        factory.ConfigureFileLogger(Path.Combine(Path.GetTempPath(), Path.GetRandomFileName()));

        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        Assert.IsNotNull(logger);
        Assert.AreEqual(nameof(LogFactoryTests), logger.ClassName);
    }
}
