using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void CreateLogger_WithoutBeingConfigured_ReturnsNull()
    {
        // req4:
        // Testing for: If the file logger has not be configured in the LogFactory, its CreateLogger method should return null
        // Arrange
        LogFactory factory = new();
        // Act
        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));
        // Assert
        Assert.IsNull(logger);
    }

    [TestMethod]
    public void CreateLogger_ProperConfiguration_ReturnsLogger()
    {
        // Testing for: The LogFactory should be updated with a new method ConfigureFileLogger.
        // This should take in a file path and store it in a private member.
        // It should use this when instantiating a new FileLogger in its CreateLogger method
        // Arrange
        LogFactory factory = new();
        string expectedPath = "log/output.txt";
        factory.ConfigureFileLogger(expectedPath);
        // Act
        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));
        // Assert
        Assert.IsNotNull(logger);
        Assert.IsInstanceOfType(logger, typeof(FileLogger));
        Assert.AreEqual(nameof(LogFactoryTests), logger.ClassName);

        var fileLogger = logger as FileLogger;
        Assert.AreEqual(expectedPath, fileLogger?.FilePath);
    }
}
