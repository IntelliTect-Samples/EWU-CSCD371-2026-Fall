using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

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
        Assert.IsTrue(output.Contains(message));
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

        // Assert
        Assert.IsTrue(output.Contains("Information"));
        Assert.IsTrue(output.Contains("Debug"));
        Assert.IsTrue(output.Contains("Error"));
    }
}