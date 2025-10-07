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
    public class ConsoleLoggerTests
    {
        [TestMethod]
        public void Error_LogToConsole_FormatsCorrectly()
        {
            // Arrange
            ConsoleLogger logger = new() { ClassName = nameof(ConsoleLoggerTests) };
            string message = "Test Message";

            using var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            logger.Error(message);

            // Assert
            string output = sw.ToString().Trim();

            var timestampToken = output.Split(']')[0].Trim('[', ']');
            Assert.IsTrue(DateTime.TryParse(timestampToken, out _), "Timestamp is not a valid DateTime");

            Assert.IsTrue(output.Contains($"[{nameof(ConsoleLoggerTests)}]"), "Class name missing or incorrect");

            Assert.IsTrue(output.Contains("[Error]"), "Log level missing or incorrect");

            Assert.IsTrue(output.Contains(message), "Log message missing or incorrect");
        }
    }
}
