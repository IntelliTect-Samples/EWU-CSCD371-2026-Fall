namespace Logger.Tests;

/// <summary>
/// Tests for log factory
/// </summary>
[TestClass]
public class LogFactoryTests
{
    /// <summary>
    /// Used for testing CreateLogger
    /// </summary>
    [TestMethod]
    public void CreateLoggerReturnsNullWhenNotConfigured()
    {
        Assert.IsNull(new LogFactory().CreateLogger(nameof(LogFactoryTests)));
    }

    /// <summary>
    /// Used for testing CreateLogger
    /// </summary>
    [TestMethod]
    public void CreateLoggerReturnsFileLoggerWithClassNameWhenConfigured()
    {
        var factory = new LogFactory();
        factory.ConfigureFileLogger("log.txt");

        BaseLogger? logger = factory.CreateLogger(nameof(LogFactoryTests));

        Assert.IsNotNull(logger);
        Assert.IsInstanceOfType<FileLogger>(logger);
        Assert.AreEqual(nameof(LogFactoryTests), logger.ClassName);
    }

    /// <summary>
    /// Used for testing CreateLogger exceptions
    /// </summary>
    [TestMethod]
    public void ConfigureFileLoggerThrowsOnNullOrEmptyPath()
    {
        var factory = new LogFactory();
        Assert.ThrowsExactly<ArgumentNullException>(() => factory.ConfigureFileLogger(null!));
        Assert.ThrowsExactly<ArgumentException>(() => factory.ConfigureFileLogger(" "));
    }
}