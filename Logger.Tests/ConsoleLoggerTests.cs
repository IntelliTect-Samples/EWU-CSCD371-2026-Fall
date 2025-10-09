using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests
{
    [TestClass]
    public class ConsoleLoggerTests : IDisposable
    {
        private StringWriter _stringWriter = null!;
        private TextWriter _output = null!;
        private bool _disposed;

        [TestInitialize]
        public void Setup()
        {
            _stringWriter = new StringWriter();
            _output = Console.Out;
            Console.SetOut(_stringWriter);
        }

        [TestCleanup]
        public void Cleanup()
        {
            Console.SetOut(_output);
            _stringWriter.Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _stringWriter?.Dispose();
                _disposed = true;
            }
            GC.SuppressFinalize(this);
        }

        [TestMethod]
        public void ConsoleLogger_Instantiate_Success()
        {
            //Arrange
            //Act
            var logger = new ConsoleLogger();
            //Assert
            Assert.IsNotNull(logger);
            Assert.IsInstanceOfType<ConsoleLogger>(logger);
        }

        [TestMethod]
        public void Log_WhenCalled_WritesExpectedOutputToConsole()
        {
            var logger = new ConsoleLogger
            {
                ClassName = nameof(ConsoleLoggerTests)
            };

            logger.Log(LogLevel.Information, "message");

            string output = _stringWriter.ToString();
            Assert.IsTrue(output.Contains("ConsoleLoggerTests"));
            Assert.IsTrue(output.Contains("Information"));
            Assert.IsTrue(output.Contains("message"));
        }

        [TestMethod]
        public void ClassName_ReturnsCorrectName()
        {
            // Arrange
            var logger = new ConsoleLogger();

            // Act
            var className = logger.ClassName;

            // Assert
            Assert.AreEqual(nameof(ConsoleLogger), className);
        }
    }
}
