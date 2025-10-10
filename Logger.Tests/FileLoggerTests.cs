using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    [TestMethod]
    public void Log_WithMessage_WritesToFile()
    {
        // Arrange
        string filePath = Path.GetTempFileName();
        var logger = new FileLogger(filePath);

        try
        {
            // Act
            logger.Log(LogLevel.Warning, "Warning message");

            // Assert
            string fileInfo = File.ReadAllText(filePath);
            Assert.IsTrue(fileInfo.Contains("Warning message"));
        }
        finally
        {
            // Cleanup
            File.Delete(filePath);
        }
    }

    [TestMethod]
    public void Log_AppendsMessage_ToNewLine()
    {
        // Arrange
        string filePath = Path.GetTempFileName();
        var logger = new FileLogger(filePath);

        try
        {
            // Act
            logger.Log(LogLevel.Error, "First Line Message");
            logger.Log(LogLevel.Error, "Second Line Message");

            // Assert
            string[] lines = File.ReadAllLines(filePath);
            Assert.AreEqual(lines.Length, 2);
            Assert.IsTrue(lines[0].Contains("First Line Message"));
            Assert.IsTrue(lines[1].Contains("Second Line Message"));
        }
        finally
        {
            // Cleanup
            File.Delete(filePath);
        }
    }

    [TestMethod]
    public void Log_FormattingDateTime_ExpectedOutput()
    {
        // Arrange
        string filePath = Path.GetTempFileName();
        var logger = new FileLogger(filePath)
        {
            ClassName = nameof(FileLoggerTests)
        };

        try
        {
            // Act
            logger.Log(LogLevel.Information, "Formatted message");

            // Assert
            string fileInfo = File.ReadAllText(filePath);
            int colonIndex = fileInfo.IndexOf(": ", StringComparison.Ordinal);
            Assert.IsTrue(colonIndex > 0, "No colon found after timestamp.");

            string timestamp = fileInfo.Substring(0, colonIndex);
            Assert.IsTrue(DateTime.TryParse(timestamp, out _), "Timestamp format is incorrect.");
        }
        finally
        {
            //Cleanup
            File.Delete(filePath);
        }
    }
}

