using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void CreateLogger_ReturnsNull_WhenFileLoggerNotConfigured()
    {
        // Arrange
        var factory = new LogFactory();

        // Act
        var logger = factory.CreateLogger(nameof(LogFactoryTests));

        // Assert
        Assert.IsNull(logger);
    }

    [TestMethod]
    public void CreateLogger_ReturnsFileLogger_WhenConfigured()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
        var factory = new LogFactory();
        factory.ConfigureFileLogger(filePath);

        try
        {
            // Act
            var logger = factory.CreateLogger(nameof(LogFactoryTests));

            // Assert
            Assert.IsNotNull(logger);
            Assert.IsInstanceOfType<FileLogger>(logger);
            Assert.AreEqual(nameof(LogFactoryTests), logger!.className);
        }
        finally
        {
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
        }
    }

    [TestMethod]
    public void CreateLogger_UsesConfiguredFilePath()
    {
        // Arrange
        var filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.txt");
        var factory = new LogFactory();
        factory.ConfigureFileLogger(filePath);

        try
        {
            // Act
            var logger = factory.CreateLogger(nameof(LogFactoryTests));
            logger!.Log(LogLevel.Information, "Factory test message");

            // Assert
            Assert.IsTrue(File.Exists(filePath));
            var contents = File.ReadAllText(filePath);
            StringAssert.Contains(contents, "Factory test message");
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
