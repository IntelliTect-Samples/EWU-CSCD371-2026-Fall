using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{

    [TestMethod]
    public void CreateLogger_WithoutConfiguration_ReturnsNull()
    {
        var factory = new Logger.LogFactory();
        var logger = factory.CreateLogger(nameof(LogFactoryTests));
        Assert.IsNull(logger);
    }

    [TestMethod]
    public void CreateLogger_WithConfiguration_ReturnsFileLogger()
    {
        var factory = new Logger.LogFactory();
        var tempPath = System.IO.Path.Combine(System.IO.Path.GetTempPath(), System.IO.Path.GetRandomFileName());
        factory.ConfigureFileLogger(tempPath);

        var logger = factory.CreateLogger(nameof(LogFactoryTests));

        Assert.IsNotNull(logger);
        Assert.IsInstanceOfType<Logger.FileLogger>(logger);
        Assert.AreEqual(nameof(LogFactoryTests), logger!.ClassName);
    }

}
