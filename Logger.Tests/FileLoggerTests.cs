
using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    private readonly string _testFilePath = "testlog.txt";
    [TestInitialize]
    public void TestInitiallize()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath); 
        }
    }

    [TestMethod]
    public void Log_WritesMessageToFile()
    {
        // Arrange
        var logger = new FileLogger(_testFilePath)
        {
            ClassName = "TestClass"
        };
        string message = "Hello world";
        LogLevel level = LogLevel.Information;

        // Act
        logger.Log(level, message);

        // Assert
        Assert.IsTrue(File.Exists(_testFilePath), "Log file was not created.");
        string[] lines = File.ReadAllLines(_testFilePath);
        Assert.AreEqual(1, lines.Length, "Log file does not contain exactly one line.");
        StringAssert.Contains(lines[0], message, "Log line does not contain the message.");
        StringAssert.Contains(lines[0], level.ToString(), "Log line does not contain the log level.");
        StringAssert.Contains(lines[0], "TestClass", "Log line does not contain the class name.");
    }

    [TestCleanup]
    public void TestCleanUp()
    {
        if (File.Exists(_testFilePath))
        {
            File.Delete(_testFilePath);
        }
    }
}
