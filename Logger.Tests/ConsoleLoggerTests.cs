using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class ConsoleLoggerTests : IDisposable
{
    private StringWriter _stringWriter = null!;
    private TextWriter _originalOutput = null!;
    private bool _disposed;

    [TestInitialize]
    public void Setup()
    {
        _stringWriter = new StringWriter();
        _originalOutput = Console.Out;
        Console.SetOut(_stringWriter);
    }

    [TestCleanup]
    public void Cleanup()
    {
        Console.SetOut(_originalOutput);
        _stringWriter.Dispose();
    }

    [TestMethod]
    public void Log_WritesToConsole()
    {
        var logger = new ConsoleLogger
        {
            ClassName = nameof(ConsoleLoggerTests)
        };

        logger.Log(LogLevel.Information, "Test message");

        string output = _stringWriter.ToString();
        Assert.IsTrue(output.Contains("ConsoleLoggerTests"));
        Assert.IsTrue(output.Contains("Information"));
        Assert.IsTrue(output.Contains("Test message"));
    }

    [TestMethod]
    public void Log_WithDifferentLogLevels_WritesCorrectLevel()
    {
        var logger = new ConsoleLogger
        {
            ClassName = nameof(ConsoleLoggerTests)
        };

        logger.Log(LogLevel.Error, "Error message");
        logger.Log(LogLevel.Warning, "Warning message");
        logger.Log(LogLevel.Debug, "Debug message");

        string output = _stringWriter.ToString();
        Assert.IsTrue(output.Contains("Error: Error message"));
        Assert.IsTrue(output.Contains("Warning: Warning message"));
        Assert.IsTrue(output.Contains("Debug: Debug message"));
    }

    [TestMethod]
    public void Log_WithExtensionMethods_WritesToConsole()
    {
        var logger = new ConsoleLogger
        {
            ClassName = nameof(ConsoleLoggerTests)
        };

        logger.Error("Error {0}", 123);
        logger.Warning("Warning {0}", "test");
        logger.Information("Info {0}", 456);
        logger.Debug("Debug {0}", true);

        string output = _stringWriter.ToString();
        Assert.IsTrue(output.Contains("Error 123"));
        Assert.IsTrue(output.Contains("Warning test"));
        Assert.IsTrue(output.Contains("Info 456"));
        Assert.IsTrue(output.Contains("Debug True"));
    }

    [TestMethod]
    public void Log_IncludesTimestamp()
    {
        var logger = new ConsoleLogger
        {
            ClassName = nameof(ConsoleLoggerTests)
        };

        logger.Log(LogLevel.Information, "Test message");

        string output = _stringWriter.ToString();
        Assert.IsTrue(output.Contains('/'));
        Assert.IsTrue(output.Contains(':'));
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _stringWriter?.Dispose();
            }

            _disposed = true;
        }
    }
}
