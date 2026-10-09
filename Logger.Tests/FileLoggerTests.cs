using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    private string? _filePath;

    [TestInitialize]
    public void Initialize()
    {
        _filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
    }

    [TestCleanup]
    public void Cleanup()
    {
        if (_filePath is not null && File.Exists(_filePath))
        {
            File.Delete(_filePath);
        }
    }

    [TestMethod]
    public void Log_CalledTwice_AppendsOnSeparateLines()
    {
        Assert.IsNotNull(_filePath);
        string filePath = _filePath;
        var logger = new FileLogger(filePath) { ClassName = nameof(FileLoggerTests) };

        logger.Log(LogLevel.Warning, "Hello");
        logger.Log(LogLevel.Warning, "World");

        string[] lines = File.ReadAllLines(filePath);

        Assert.HasCount(2, lines);
        Assert.Contains("Hello", lines[0]);
        Assert.Contains("World", lines[1]);
    }

    [TestMethod]
    public void Log_ExistingFileContents_AppendsOnNewLine()
    {
        Assert.IsNotNull(_filePath);
        string filePath = _filePath;
        var logger = new FileLogger(filePath) { ClassName = nameof(FileLoggerTests) };

        File.WriteAllText(filePath, "Existing" + Environment.NewLine);
        logger.Log(LogLevel.Warning, "Hello");
        string[] lines = File.ReadAllLines(filePath);

        Assert.HasCount(2, lines);
        Assert.AreEqual("Existing", lines[0]);
        Assert.Contains("Hello", lines[1]);
    }

    [TestMethod]
    public void Log_FileDoesNotExist_CreatesFileAndWritesMessage()
    {
        Assert.IsNotNull(_filePath);
        string filePath = _filePath;
        Assert.IsFalse(File.Exists(filePath));

        var logger = new FileLogger(filePath) { ClassName = nameof(FileLoggerTests) };
        logger.Log(LogLevel.Warning, "Hello");

        Assert.IsTrue(File.Exists(filePath));
        Assert.Contains("Hello", File.ReadAllText(filePath));
    }
}
