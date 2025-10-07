using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    [TestMethod]
    public void Error_LogToFile_FormatsCorrectly()
    {
        // Arrange
        string tempFilePath = Path.GetTempFileName();
        FileLogger logger = new(tempFilePath) { ClassName = nameof(FileLoggerTests) };
        string message = "Test Message";

        // Act
        logger.Error(message);

        // Assert
        string[] lines = File.ReadAllLines(tempFilePath);
        Assert.AreEqual(1, lines.Length, "Expected exactly one log entry");

        string output = lines[0];

        // Extract timestamp (assumes format "M/d/yyyy h:mm:ss tt")
        string[] tokens = output.Split(' ');
        string timestamp = $"{tokens[0]} {tokens[1]} {tokens[2]}";
        Assert.IsTrue(DateTime.TryParse(timestamp, out _), "Timestamp is not a valid DateTime");

        Assert.IsTrue(output.Contains(nameof(FileLoggerTests)), "Class name missing or incorrect");
        Assert.IsTrue(output.Contains("Error"), "Log level missing or incorrect");
        Assert.IsTrue(output.Contains(message), "Log message missing or incorrect");

        // Cleanup
        File.Delete(tempFilePath);
    }
}
