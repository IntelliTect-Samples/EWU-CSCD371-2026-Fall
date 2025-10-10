using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void CreateLogger_WithoutFilePath_ReturnNull()
    {
        // Arrange
        LogFactory factory = new LogFactory();

        // Act
        BaseLogger? logger = factory.CreateLogger("TestClass");

        // Assert
        Assert.IsNull(logger, "Logger should be null before configuration.");
    }

    [TestMethod]
    public void CreateLogger_AfterConfigure_ReturnLogger()
    {
        // Arrange
        LogFactory factory = new LogFactory();
        factory.ConfigureFileLogger("logs/log.txt");

        // Act
        BaseLogger? logger = factory.CreateLogger("TestClass"); 

        // Assert
        Assert.IsNotNull(logger, "Logger should not be null after configuration.");
        Assert.AreEqual("TestClass", logger.ClassName);
    }
    
}
