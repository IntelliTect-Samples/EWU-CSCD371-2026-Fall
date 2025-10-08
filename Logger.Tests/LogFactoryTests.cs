using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Logging;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void CreateLogger_FileLogger_Success()
    {
        LogFactory factory = new();
        factory.ConfigureFileLogger("test.log");
        var logger = factory.CreateLogger(nameof(LogFactoryTests));
        Assert.AreEqual(nameof(LogFactoryTests), logger?.ClassName);
    }

    [TestMethod]
    public void CreateLogger_FileLogger_NullFilePath()
    {
        LogFactory factory = new();
        var logger = factory.CreateLogger(nameof(LogFactoryTests));
        Assert.IsNull(logger);
    }


    [TestMethod]
    public void ConfigureFileLogger_ValidPath_CreatesFileLogger()
    {
        const string logPath = "factory_test.log";
        if (File.Exists(logPath))
        {
            File.Delete(logPath);
        }

        LogFactory factory = new();
        factory.ConfigureFileLogger(logPath);

        var logger = factory.CreateLogger(nameof(LogFactoryTests));

        Assert.IsNotNull(logger);
        Assert.AreEqual(nameof(LogFactoryTests), logger.ClassName);
    }


    [TestMethod]
    public void CreateLogger_DifferentClassNames_AssignsCorrectClassNames()
    {
        LogFactory factory = new();
        factory.ConfigureFileLogger("test.log");

        var loggerA = factory.CreateLogger("ClassA");
        var loggerB = factory.CreateLogger("ClassB");

        Assert.AreEqual("ClassA", loggerA?.ClassName);
        Assert.AreEqual("ClassB", loggerB?.ClassName);
    }
}
