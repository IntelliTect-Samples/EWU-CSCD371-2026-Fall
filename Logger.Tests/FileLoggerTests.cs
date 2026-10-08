using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.Globalization;
using System.IO;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    private string _filePath = null!;

    [TestInitialize]
    public void TestInitialize()
    {
        _filePath = Path.GetTempFileName();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        if (File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }

    [TestMethod]
    public void Constructor_WithFilePath_SetsFilePath()
    {
        var logger = new FileLogger(_filePath) { ClassName = nameof(FileLoggerTests) };

        Assert.AreEqual(_filePath, logger.FilePath);
    }

    [TestMethod]
    public void Log_WritesDateClassNameLevelAndMessage()
    {
        // Arrange
        var logger = new FileLogger(_filePath) { ClassName = nameof(FileLoggerTests) };
        string expectedSuffix = $" {nameof(FileLoggerTests)} Warning: Test message";
        // The logged timestamp is truncated to whole seconds, so allow up to a second before.
        DateTime before = DateTime.Now.AddSeconds(-1);

        // Act
        logger.Log(LogLevel.Warning, "Test message");
        DateTime after = DateTime.Now;

        // Assert
        string[] lines = File.ReadAllLines(_filePath);
        Assert.HasCount(1, lines);
        Assert.EndsWith(expectedSuffix, lines[0]);
        string timestamp = lines[0][..^expectedSuffix.Length];
        DateTime loggedAt = DateTime.Parse(timestamp, CultureInfo.CurrentCulture);
        Assert.IsInRange(before, after, loggedAt);
    }

    [TestMethod]
    public void Log_CalledTwice_AppendsEachMessageOnItsOwnLine()
    {
        var logger = new FileLogger(_filePath) { ClassName = nameof(FileLoggerTests) };

        logger.Log(LogLevel.Information, "First");
        logger.Log(LogLevel.Error, "Second");

        string[] lines = File.ReadAllLines(_filePath);
        Assert.HasCount(2, lines);
        Assert.EndsWith("Information: First", lines[0]);
        Assert.EndsWith("Error: Second", lines[1]);
    }

    [TestMethod]
    public void Log_FileAlreadyHasContent_DoesNotOverwrite()
    {
        File.WriteAllText(_filePath, "Existing line" + Environment.NewLine);
        var logger = new FileLogger(_filePath) { ClassName = nameof(FileLoggerTests) };

        logger.Log(LogLevel.Debug, "New line");

        string[] lines = File.ReadAllLines(_filePath);
        Assert.HasCount(2, lines);
        Assert.AreEqual("Existing line", lines[0]);
        Assert.EndsWith("Debug: New line", lines[1]);
    }

    [TestMethod]
    public void Log_ThroughExtensionMethod_WritesFormattedMessage()
    {
        var logger = new FileLogger(_filePath) { ClassName = nameof(FileLoggerTests) };

        logger.Error("Failed after {0} tries", 3);

        string[] lines = File.ReadAllLines(_filePath);
        Assert.HasCount(1, lines);
        Assert.EndsWith($"{nameof(FileLoggerTests)} Error: Failed after 3 tries", lines[0]);
    }
}
