using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    private string _testFilePath = null!;

    [TestInitialize]
    public void Setup()
    {
        // Create a unique temporary file path for each test
        _testFilePath = Path.Combine(Path.GetTempPath(), $"test_log_{Guid.NewGuid()}.txt");
    }

    [TestCleanup]
    public void Cleanup()
    {
        // Clean up the test file after each test
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }

    [TestMethod]
    public void Log_WritesToFile()
    {
        // Arrange
        var logger = new FileLogger(_testFilePath)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Act
        logger.Log(LogLevel.Information, "Test message");

        // Assert
        Assert.IsTrue(File.Exists(_testFilePath));
        string content = File.ReadAllText(_testFilePath);
        Assert.IsTrue(content.Contains("FileLoggerTests"));
        Assert.IsTrue(content.Contains("Information"));
        Assert.IsTrue(content.Contains("Test message"));
    }

    [TestMethod]
    public void Log_WithMultipleMessages_AppendsToFile()
    {
        // Arrange
        var logger = new FileLogger(_testFilePath)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Act
        logger.Log(LogLevel.Error, "First message");
        logger.Log(LogLevel.Warning, "Second message");
        logger.Log(LogLevel.Debug, "Third message");

        // Assert
        string content = File.ReadAllText(_testFilePath);
        Assert.IsTrue(content.Contains("First message"));
        Assert.IsTrue(content.Contains("Second message"));
        Assert.IsTrue(content.Contains("Third message"));
        
        // Verify each message is on its own line
        string[] lines = content.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
        Assert.AreEqual(3, lines.Length);
    }

    [TestMethod]
    public void Log_WithDifferentLogLevels_WritesCorrectLevel()
    {
        // Arrange
        var logger = new FileLogger(_testFilePath)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Act
        logger.Log(LogLevel.Error, "Error message");
        logger.Log(LogLevel.Warning, "Warning message");
        logger.Log(LogLevel.Information, "Info message");
        logger.Log(LogLevel.Debug, "Debug message");

        // Assert
        string content = File.ReadAllText(_testFilePath);
        Assert.IsTrue(content.Contains("Error: Error message"));
        Assert.IsTrue(content.Contains("Warning: Warning message"));
        Assert.IsTrue(content.Contains("Information: Info message"));
        Assert.IsTrue(content.Contains("Debug: Debug message"));
    }

    [TestMethod]
    public void Log_WithExtensionMethods_WritesToFile()
    {
        // Arrange
        var logger = new FileLogger(_testFilePath)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Act
        logger.Error("Error {0}", 123);
        logger.Warning("Warning {0}", "test");
        logger.Information("Info {0}", 456);
        logger.Debug("Debug {0}", true);

        // Assert
        string content = File.ReadAllText(_testFilePath);
        Assert.IsTrue(content.Contains("Error 123"));
        Assert.IsTrue(content.Contains("Warning test"));
        Assert.IsTrue(content.Contains("Info 456"));
        Assert.IsTrue(content.Contains("Debug True"));
    }

    [TestMethod]
    public void Log_IncludesTimestamp()
    {
        // Arrange
        var logger = new FileLogger(_testFilePath)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Act
        logger.Log(LogLevel.Information, "Test message");

        // Assert
        string content = File.ReadAllText(_testFilePath);

        // Timestamp should contain date separator (/) and time separator (:)
        Assert.IsTrue(content.Contains('/'));
        Assert.IsTrue(content.Contains(':'));
    }

    [TestMethod]
    public void FilePath_PropertyReturnsCorrectPath()
    {
        // Arrange & Act
        var logger = new FileLogger(_testFilePath);

        // Assert
        Assert.AreEqual(_testFilePath, logger.FilePath);
    }

    [TestMethod]
    public void Log_CreatesFileIfNotExists()
    {
        // Arrange
        var logger = new FileLogger(_testFilePath)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Ensure file doesn't exist before logging
        Assert.IsFalse(File.Exists(_testFilePath));

        // Act
        logger.Log(LogLevel.Information, "Test message");

        // Assert
        Assert.IsTrue(File.Exists(_testFilePath));
    }

    [TestMethod]
    public void Log_WithFormattedMessage_WritesFormattedContent()
    {
        // Arrange
        var logger = new FileLogger(_testFilePath)
        {
            ClassName = nameof(FileLoggerTests)
        };

        // Act
        logger.Information("User {0} logged in at {1}", "JohnDoe", DateTime.Now.ToShortTimeString());

        // Assert
        string content = File.ReadAllText(_testFilePath);
        Assert.IsTrue(content.Contains("User JohnDoe logged in"));
    }
}