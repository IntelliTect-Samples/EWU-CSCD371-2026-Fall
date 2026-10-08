using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    [TestMethod]
    public void Log_WritesTimestampClassNameLevelAndMessage()
    {
        string filePath = CreateTempFilePath();

        try
        {
            var logger = new FileLogger(filePath)
            {
                ClassName = nameof(FileLoggerTests)
            };

            DateTime before = DateTime.Now;

            logger.Log(LogLevel.Warning, "Test message");

            DateTime after = DateTime.Now;

            string[] lines = File.ReadAllLines(filePath);

            Assert.HasCount(1, lines);

            string loggedMessage = lines[0];
            string classNameMarker = $" {nameof(FileLoggerTests)} ";

            int classNameIndex = loggedMessage.IndexOf(
                classNameMarker,
                StringComparison.CurrentCulture);

            Assert.IsTrue(classNameIndex >= 0);

            string timestampText = loggedMessage[..classNameIndex];

            Assert.IsTrue(
                DateTime.TryParse(
                    timestampText,
                    CultureInfo.CurrentCulture,
                    DateTimeStyles.None,
                    out DateTime timestamp));

            Assert.IsTrue(timestamp >= before.AddSeconds(-1));
            Assert.IsTrue(timestamp <= after.AddSeconds(1));
            Assert.IsTrue(loggedMessage.Contains(nameof(FileLoggerTests)));
            Assert.IsTrue(loggedMessage.Contains("Warning"));
            Assert.IsTrue(loggedMessage.Contains("Test message"));
        }
        finally
        {
            DeleteTempFile(filePath);
        }
    }

    [TestMethod]
    public void Log_AppendsMessagesToFile()
    {
        string filePath = CreateTempFilePath();

        try
        {
            var logger = new FileLogger(filePath)
            {
                ClassName = nameof(FileLoggerTests)
            };

            logger.Log(LogLevel.Information, "First message");
            logger.Log(LogLevel.Debug, "Second message");

            string[] lines = File.ReadAllLines(filePath);

            Assert.HasCount(2, lines);
            Assert.IsTrue(lines[0].Contains("First message"));
            Assert.IsTrue(lines[1].Contains("Second message"));
            Assert.IsTrue(lines[0].Contains("Information"));
            Assert.IsTrue(lines[1].Contains("Debug"));
        }
        finally
        {
            DeleteTempFile(filePath);
        }
    }

    private static string CreateTempFilePath()
    {
        return Path.Combine(
            Path.GetTempPath(),
            $"{Guid.NewGuid():N}.log");
    }

    private static void DeleteTempFile(string filePath)
    {
        if (File.Exists(filePath))
        {
            File.Delete(filePath);
        }
    }
}