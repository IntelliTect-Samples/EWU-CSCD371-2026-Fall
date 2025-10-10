using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;

namespace Logger.Tests;


[TestClass]
public class FileLoggerTests
{
    private const string TestFilePath = "./testlog.txt";
    private const string LoggerClassName = "TestFileLogger";

    private static void CleanUpFile()
    {
        if (File.Exists(TestFilePath))
        {
            File.Delete(TestFilePath);
        }
    }

    [TestInitialize]
    public void TestInitialize()
    {
        CleanUpFile();
    }

    [TestCleanup]
    public void TestCleanup()
    {
        CleanUpFile();
    }

    private static FileLogger CreateFileLogger()
    {
        return new FileLogger(TestFilePath) { LoggerClassName = LoggerClassName };
    }

    [TestMethod]
    public void FileLogger_Initializes_Success()
    {
        // act
        FileLogger fileLogger = CreateFileLogger();
        // assert
        Assert.AreEqual(TestFilePath, fileLogger.FilePath);
        Assert.AreEqual(LoggerClassName, fileLogger.LoggerClassName);
    }

    [TestMethod]
    public void Log_CreatesFileIfNotExisting_Success()
    {
        // arrange
        string testMessage = "This is a test log message.";
        FileLogger fileLogger = CreateFileLogger();
        // act
        fileLogger.Log(LogLevel.Information, testMessage);
        // assert
        Assert.IsTrue(File.Exists(TestFilePath), "File was not created by FileLogger");
    }

    [TestMethod]
    public void Log_AppendsMessageToFile_Success()
    {
        // arrange
        DateTime now = DateTime.Now;
        string testMessage1 = "This is the first test log message.";
        string testMessage2 = "This is the second test log message.";
        string expectedLogMessage1 = $"[{LoggerClassName}] [Information] {testMessage1}";
        string expectedLogMessage2 = $"[{LoggerClassName}] [Information] {testMessage2}";
        FileLogger fileLogger = CreateFileLogger();
        // act
        fileLogger.Log(LogLevel.Information, testMessage1);
        fileLogger.Log(LogLevel.Information, testMessage2);
        // assert
        string[] logContents = File.ReadAllLines(TestFilePath);

        int idx0 = logContents[0].IndexOf(']');
        string log_0 = idx0 >= 0 && idx0 + 2 <= logContents[0].Length
            ? logContents[0].Substring(idx0 + 2)
            : logContents[0];

        int idx1 = logContents[1].IndexOf(']');
        string log_1 = idx1 >= 0 && idx1 + 2 <= logContents[1].Length
            ? logContents[1].Substring(idx1 + 2)
            : logContents[1];

        Assert.AreEqual(2, logContents.Length, "Log file does not contain expected number of entries");
        Assert.AreEqual(expectedLogMessage1, log_0, "First log message does not match");
        Assert.AreEqual(expectedLogMessage2, log_1, "Second log message does not match");
    }

    [TestMethod]
    public void Log_MessageIsCorrect_Success()
    { 
        // arrange
        DateTime now = DateTime.Now;
        string testMessage = "This is a test log message.";
        string expectedLogMessage = $"[{LoggerClassName}] [Information] {testMessage}";
        FileLogger fileLogger = CreateFileLogger();
        // act
        fileLogger.Log(LogLevel.Information, testMessage);
        // assert
        string[] logContents = File.ReadAllLines(TestFilePath);

        int idx = logContents[0].IndexOf(']');
        string log_0 = idx >= 0 && idx + 2 <= logContents[0].Length
            ? logContents[0].Substring(idx + 2)
            : logContents[0];

        Assert.AreEqual(expectedLogMessage, log_0);
    }
}
