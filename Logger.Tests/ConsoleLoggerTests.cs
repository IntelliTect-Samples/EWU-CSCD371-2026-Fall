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
        var logFactory = new LogFactory();
        var newLogger = logFactory.CreateLogger<ConsoleLogger>(nameof(ConsoleLoggerTests))!;
        var message = "Log Message";
        using var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        // Act
        newLogger.Information(message);
        Console.SetOut(originalOut);
        var output = sw.ToString();

        // Assert
        Assert.IsTrue(output.Contains(message));
    }
    [TestMethod]
    public void Log_MultipleLogs_Success()
    {
        // Arrange
        var logFactory = new LogFactory();
        var newLogger = logFactory.CreateLogger<ConsoleLogger>(nameof(ConsoleLoggerTests))!;
        var message = "Log Message";
        using var sw = new StringWriter();
        var originalOut = Console.Out;
        Console.SetOut(sw);

        // Act
        newLogger.Information(message);
        newLogger.Debug(message);
        newLogger.Error(message);
        Console.SetOut(originalOut);
        var output = sw.ToString();

        // Assert
        Assert.IsTrue(output.Contains("Information"));
        Assert.IsTrue(output.Contains("Debug"));
        Assert.IsTrue(output.Contains("Error"));
    }
}