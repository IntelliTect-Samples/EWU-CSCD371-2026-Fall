using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void CreateLogger_WithoutConfiguration_ReturnsNull()
    {
        LogFactory factory = new LogFactory();

        BaseLogger? logger = factory.CreateLogger("TestClass");

        Assert.IsNull(logger);
    }

    [TestMethod]
    public void CreateLogger_WithFileConfiguration_ReturnsFileLogger()
    {
        LogFactory factory = new LogFactory();

        factory.ConfigureFileLogger("test.txt");

        BaseLogger? logger = factory.CreateLogger("TestClass");

        Assert.IsInstanceOfType<FileLogger>(logger);
    }

    [TestMethod]
    public void CreateLogger_SetsClassName()
    {
        // Arrange
        LogFactory factory = new LogFactory();
        factory.ConfigureFileLogger("test.txt");

        // Act
        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNotNull(logger);
        Assert.AreEqual(nameof(LogFactoryTests), logger!.ClassName);
    }
}
