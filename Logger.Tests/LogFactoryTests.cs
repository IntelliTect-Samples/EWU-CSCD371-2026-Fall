using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using Logger;

namespace Logger.Tests
{
    [TestClass]
    public class LogFactoryTests
    {
        private LogFactory _logFactory = null!;
        private const string TestClassName = nameof(LogFactoryTests);

        [TestInitialize]
        public void Setup()
        {
            _logFactory = new LogFactory();
        }

        [TestMethod]
        public void CreateLogger_ReturnsNull_WhenFileLoggerIsNotConfigured()
        {
            // Arrange

            // Act
            var logger = _logFactory.CreateLogger(TestClassName);

            // Assert
            Assert.IsNull(logger, "Error: CreateLogger must return null if ConfigureFileLogger was not called.");
        }

        [TestMethod]
        public void CreateLogger_ReturnsFileLogger_WhenConfigured()
        {
            // Arrange
            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);
            _logFactory.ConfigureFileLogger(tempFilePath);

            // Act
            var logger = _logFactory.CreateLogger(TestClassName);

            // Assert
            Assert.IsNotNull(logger, "Error: CreateLogger must return a logger instance after configuration.");
            Assert.IsTrue(logger is FileLogger, "Error: The returned logger instance must be of type FileLogger.");
            File.Delete(tempFilePath);
        }

        [TestMethod]
        public void CreateLogger_PassesClassNameToFileLogger_WhenConfigured()
        {
            // Arrange
            string tempFilePath = Path.GetTempFileName();
            File.Delete(tempFilePath);
            _logFactory.ConfigureFileLogger(tempFilePath);
            const string expectedClassName = "TestLoggerClassName";

            // Act
            var logger = _logFactory.CreateLogger(expectedClassName) as FileLogger;

            // Assert
            Assert.IsNotNull(logger, "Error: FileLogger should not be null.");
            Assert.AreEqual(expectedClassName, logger.ClassName, "Error: The ClassName of the FileLogger must match the name associated with CreateLogger.");
            File.Delete(tempFilePath);
        }

        [TestMethod]
        public void ConfigureFileLogger_ThrowsException_ForNullPath()
        {
            // Arrange & Act
            Assert.ThrowsExactly<ArgumentException>(() => _logFactory.ConfigureFileLogger(null!)); 
        }

        [TestMethod]
        public void ConfigureFileLogger_ThrowsException_ForEmptyPath()
        {
            // Arrange & Act
            Assert.ThrowsExactly<ArgumentException>(() => _logFactory.ConfigureFileLogger(string.Empty));
        }

        [TestMethod]
        public void ConfigureFileLogger_ThrowsException_ForWhitespacePath()
        {
            // Arrange & Act
            Assert.ThrowsExactly<ArgumentException>(() => _logFactory.ConfigureFileLogger("   "));
        }

        [TestMethod]
        public void CreateLogger_ThrowsException_ForNullClassName()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _logFactory.CreateLogger(null!));
        }

        [TestMethod]
        public void CreateLogger_ThrowsException_ForEmptyClassName()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _logFactory.CreateLogger(string.Empty));
        }

        [TestMethod]
        public void CreateLogger_ThrowsException_ForWhitespaceClassName()
        {
            // Act & Assert
            Assert.ThrowsExactly<ArgumentException>(() => _logFactory.CreateLogger("   "));
        }
    }
}
