namespace Logger.Tests;

[TestClass]
public class BaseLoggerExtensionsTests
{
    private string _path = string.Empty;

    [TestInitialize]
    public void Setup()
    {
        _path = Path.GetTempFileName();
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_path != null || File.Exists(_path)) {
            File.Delete(_path);
        } 
    }

    private FileLogger CreateLogger() =>
        new(_path) { ClassName = nameof(BaseLoggerExtensionsTests) };

    // ---- Error ----

    [TestMethod]
    public void Error_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger logger = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Error("Message"));
    }

    [TestMethod]
    public void Error_WithData_LogsMessage()
    {
        // Arrange
        FileLogger logger = CreateLogger();

        // Act
        logger.Error("Message {0}", 42);

        // Assert
        string[] lines = File.ReadAllLines(_path);
        Assert.HasCount(1, lines);
        StringAssert.EndsWith(lines[0], "Error: Message 42");
    }

    // ---- Warning ----

    [TestMethod]
    public void Warning_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger logger = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Warning("Message"));
    }

    [TestMethod]
    public void Warning_WithData_LogsMessage()
    {
        // Arrange
        FileLogger logger = CreateLogger();

        // Act
        logger.Warning("Message {0}", "x");

        // Assert
        string[] lines = File.ReadAllLines(_path);
        Assert.HasCount(1, lines);
        StringAssert.EndsWith(lines[0], "Warning: Message x");
    }

    // ---- Information ----

    [TestMethod]
    public void Information_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger logger = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Information("Message"));
    }

    [TestMethod]
    public void Information_WithoutArguments_LogsMessage()
    {
        // Arrange
        FileLogger logger = CreateLogger();

        // Act
        logger.Information("Plain");

        // Assert
        string[] lines = File.ReadAllLines(_path);
        Assert.HasCount(1, lines);
        StringAssert.EndsWith(lines[0], "Information: Plain");
    }

    // ---- Debug ----

    [TestMethod]
    public void Debug_WithNullLogger_ThrowsException()
    {
        // Arrange
        BaseLogger logger = null!;

        // Act & Assert
        Assert.ThrowsExactly<ArgumentNullException>(() => logger.Debug("Message"));
    }

    [TestMethod]
    public void Debug_WithMultipleArguments_LogsMessage()
    {
        // Arrange
        FileLogger logger = CreateLogger();

        // Act
        logger.Debug("{0}-{1}", 1, 2);

        // Assert
        string[] lines = File.ReadAllLines(_path);
        Assert.HasCount(1, lines);
        StringAssert.EndsWith(lines[0], "Debug: 1-2");
    }
}