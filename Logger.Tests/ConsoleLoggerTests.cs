using System;
using System.IO;
using System.Linq;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Logger.Tests;

[TestClass]
public class ConsoleLoggerTests
{
    [TestMethod]
    public void CreateLogger_ValidCreation_IsTypeConsoleLogger()
    {
        // Arrange
        LogFactory logFactory = new();
        ConsoleLogger newLogger = logFactory.CreateLogger<ConsoleLogger>(nameof(ConsoleLoggerTests))!;
        // Act

        // Assert
        Assert.IsInstanceOfType(newLogger, typeof(ConsoleLogger));
    }

    [TestMethod]
    public void Log_ValidLog_MessageLoggedInStandardOutput()
    {
        // Arrange
        LogFactory logFactory = new ();
        ConsoleLogger newLogger = logFactory.CreateLogger<ConsoleLogger>(nameof(ConsoleLoggerTests))!;
        string message = "Log Message";
        using StringWriter sw = new ();
        TextWriter originalOut = Console.Out;
        Console.SetOut(sw);

        // Act
        newLogger.Information(message);
        Console.SetOut(originalOut);
        string output = sw.ToString();

        // Assert
        Assert.Contains(message, output);
    }
    [TestMethod]
    public void Log_MultipleLogs_Success()
    {
        // Arrange
        LogFactory logFactory = new ();
        ConsoleLogger newLogger = logFactory.CreateLogger<ConsoleLogger>(nameof(ConsoleLoggerTests))!;
        string message = "Log Message";
        using StringWriter sw = new ();
        TextWriter originalOut = Console.Out;
        Console.SetOut(sw);

        // Act
        newLogger.Information(message);
        newLogger.Debug(message);
        newLogger.Error(message);
        Console.SetOut(originalOut);
        string output = sw.ToString();
        int lineCount = output.Split(new[] { Environment.NewLine, "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries).Count();

        // Assert
        Assert.Contains("Information", output);
        Assert.Contains("Debug", output);
        Assert.Contains("Error", output);
        Assert.AreEqual(3, lineCount);
    }
}