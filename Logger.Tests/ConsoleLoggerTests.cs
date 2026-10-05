using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;

namespace Logger.Tests;

[TestClass]
[DoNotParallelize]
public class ConsoleLoggerTests
{
    [TestMethod]
    [DataRow(LogLevel.Debug, "Test Message")]
    [DataRow(LogLevel.Error, "Test Message")]
    [DataRow(LogLevel.Information, "Test Message")]
    [DataRow(LogLevel.Warning, "Test Message")]
    public void Log_WhenCalled_WritesEntryToConsole(LogLevel level, string message)
    {
        TextWriter originalOutput = Console.Out;
        using var capturedOutput = new StringWriter();

        try
        {
            Console.SetOut(capturedOutput);
            var logger = new ConsoleLogger() { ClassName = nameof(ConsoleLoggerTests) };

            DateTime before = DateTime.Now;
            logger.Log(level, message);
            DateTime after = DateTime.Now;

            using var reader = new StringReader(capturedOutput.ToString());
            string? contents = reader.ReadLine();
            Assert.IsNotNull(contents);
            Assert.IsNull(reader.ReadLine());

            Assert.Contains(message, contents);
            Assert.Contains(level.ToString(), contents);
            Assert.Contains(nameof(ConsoleLoggerTests), contents);
            int classNamePosition = contents.IndexOf(nameof(ConsoleLoggerTests), StringComparison.Ordinal);
            Assert.AreEqual($"{nameof(ConsoleLoggerTests)} {level} : {message}", contents[classNamePosition..]);
            string timestampText = contents[..classNamePosition].Trim();
            Assert.IsTrue(DateTime.TryParse(timestampText, out DateTime timestamp));
            Assert.IsTrue(timestamp >= before.AddSeconds(-1) && timestamp <= after);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

    }

    [TestMethod]
    public void Log_CalledTwice_WritesEntriesOnSeparateLines()
    {
        TextWriter originalOutput = Console.Out;
        using var capturedOutput = new StringWriter();
        try
        {
            Console.SetOut(capturedOutput);
            var logger = new ConsoleLogger() { ClassName = nameof(ConsoleLoggerTests) };

            logger.Log(LogLevel.Warning, "First Message");
            logger.Log(LogLevel.Warning, "Second Message");
            
            using var reader = new StringReader(capturedOutput.ToString());
            string? firstLine = reader.ReadLine();
            string? secondLine = reader.ReadLine();

            Assert.IsNotNull(firstLine);
            Assert.IsNotNull(secondLine);
            Assert.Contains("First Message", firstLine);
            Assert.Contains("Second Message", secondLine);
            Assert.IsNull(reader.ReadLine());
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }
}
