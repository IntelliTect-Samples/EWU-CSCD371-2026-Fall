using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Globalization;
using System.IO;
using Logger;

namespace Logger.Tests
{
    [TestClass]
    public class FileLoggerTests
    {
        private string? _tempFilePath;
        private LogFactory? _logFactory;
        private const string TestClassName = nameof(FileLoggerTests);

        [TestInitialize]
        public void Setup()
        {
            _tempFilePath = Path.GetTempFileName();
            File.Delete(_tempFilePath);
            _logFactory = new LogFactory();
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (_tempFilePath != null && File.Exists(_tempFilePath))
            {
                File.Delete(_tempFilePath);
            }
        }

        [TestMethod]
        public void Log_CreatesFileAndVerifiesContent()
        {
            // Arrange
            _logFactory!.ConfigureFileLogger(_tempFilePath!);
            var fileLogger = _logFactory.CreateLogger(TestClassName);
            const string testMessage = "Test file created.";

            // Act
            fileLogger!.Log(LogLevel.Debug, testMessage);

            // Assert
            Assert.IsTrue(File.Exists(_tempFilePath), "Error: The log file was not created.");
            string logContent = File.ReadAllText(_tempFilePath!);
            Assert.IsFalse(string.IsNullOrEmpty(logContent), "Error: Log file content is empty.");
        }

        [TestMethod]
        public void Log_WritesFullEntryInCorrectFormat()
        {
            // Arrange
            _logFactory!.ConfigureFileLogger(_tempFilePath!);
            var fileLogger = _logFactory.CreateLogger(TestClassName);
            const string testMessage = "Full format verification.";

            // Act
            fileLogger!.Log(LogLevel.Warning, testMessage);

            // Assert
            string logContent = File.ReadAllText(_tempFilePath!);
            string logLine = logContent.TrimEnd(Environment.NewLine.ToCharArray());

            StringAssert.Contains(logLine, TestClassName, "Error: The log entry must contain the class name.");
            StringAssert.Contains(logLine, "Warning", "The log entry must contain the LogLevel.");
            StringAssert.Contains(logLine, testMessage, "Error: The log entry must contain the message.");

            string[] parts = logLine.Split(' ');

            Assert.IsTrue(DateTime.TryParse(parts[0] + " " + parts[1] + " " + parts[2], CultureInfo.InvariantCulture, DateTimeStyles.None, out _),
                "Error: The log entry must start with a valid date format: (MM/dd/yyyy) and valid time format: (HH:mm:ss)");
        }

        [TestMethod]
        public void Log_AppendsFirstMessageCorrectly()
        {
            // Arrange
            _logFactory!.ConfigureFileLogger(_tempFilePath!);
            var fileLogger = _logFactory.CreateLogger(TestClassName);
            const string message1 = "This is the first log entry.";
            const string message2 = "This is the second log entry.";

            // Act
            fileLogger!.Log(LogLevel.Warning, message1);
            fileLogger.Log(LogLevel.Error, message2);

            // Assert
            string[] logLines = File.ReadAllLines(_tempFilePath!);

            Assert.AreEqual(2, logLines.Length, "Expected two log lines to be written.");

            StringAssert.Contains(logLines[0], "Warning", "First log line must contain Warning level.");
            StringAssert.Contains(logLines[0], message1, "First log line must contain message 1.");
        }

        [TestMethod]
        public void Log_AppendsSecondMessageCorrectly()
        {
            // Arrange
            _logFactory!.ConfigureFileLogger(_tempFilePath!);
            var fileLogger = _logFactory.CreateLogger(TestClassName);
            const string message1 = "First log entry.";
            const string message2 = "Second log entry.";

            // Act
            fileLogger!.Log(LogLevel.Warning, message1);
            fileLogger.Log(LogLevel.Error, message2);

            // Asserts
            string[] logLines = File.ReadAllLines(_tempFilePath!);

            Assert.AreEqual(2, logLines.Length, "Expected two log lines to be written.");
            StringAssert.Contains(logLines[1], "Error", "Second log line must contain Error level.");
            StringAssert.Contains(logLines[1], message2, "Second log line must contain message 2.");
        }

        [TestMethod]
        public void Log_HandlesDifferentLogLevels()
        {
            // Arrange
            _logFactory!.ConfigureFileLogger(_tempFilePath!);
            var fileLogger = _logFactory.CreateLogger(TestClassName);
            const string debugMessage = "This is a Debug call.";
            const string infoMessage = "This is a Info call.";

            // Act
            fileLogger!.Log(LogLevel.Debug, debugMessage);
            fileLogger.Log(LogLevel.Information, infoMessage);

            // Assert
            string logContent = File.ReadAllText(_tempFilePath!);
            StringAssert.Contains(logContent, $"Debug: {debugMessage}", "Error: The log must correctly output the Debug level and message.");
            StringAssert.Contains(logContent, $"Information: {infoMessage}", "Error: The log must correctly output the Information level and message.");
        }

        [TestMethod]
        public void FileLogger_ThrowsException_IfClassNameIsNull()
        {
            // Arrange & Act
            _logFactory?.ConfigureFileLogger(_tempFilePath!);
            Assert.ThrowsExactly<ArgumentException>(() => _logFactory?.CreateLogger(null!));
            // Assert (Handled by attribute)
        }
    }
}