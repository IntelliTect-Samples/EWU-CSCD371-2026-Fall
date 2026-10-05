using Microsoft.VisualStudio.TestTools.UnitTesting;

using System;
using System.IO;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{

    [TestMethod]
    public void Log_CalledTwice_AppendsOnSeparateLines()
    {
        string filePath = Path.GetTempFileName();
        try
        {
            var logger = new FileLogger(filePath) { ClassName = nameof(FileLoggerTests) };

            logger.Log(LogLevel.Warning, "Hello");
            logger.Log(LogLevel.Warning, "World");

            string[] lines = File.ReadAllLines(filePath);

            Assert.HasCount(2, lines);
            Assert.Contains("Hello", lines[0]);
            Assert.Contains("World", lines[1]);
        }
        finally
        {
            File.Delete(filePath);
        }
    }

    [TestMethod]
    public void Log_ExistingFileContents_AppendsOnNewLine()
    {
        string filePath = Path.GetTempFileName();
        try
        {
            var logger = new FileLogger(filePath) { ClassName = nameof(FileLoggerTests) };

            File.WriteAllText(filePath, "Existing" + Environment.NewLine);
            logger.Log(LogLevel.Warning, "Hello");
            string[] lines = File.ReadAllLines(filePath);

            Assert.HasCount(2, lines);
            Assert.AreEqual("Existing", lines[0]);
            Assert.Contains("Hello", lines[1]);
        }
        finally
        {
            File.Delete(filePath);
        }

    }

    [TestMethod]
    public void Log_FileDoesNotExist_CreatesFileAndWritesMessage()
    {
        string filePath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        try
        {
            Assert.IsFalse(File.Exists(filePath));

            var logger = new FileLogger(filePath) { ClassName = nameof(FileLoggerTests) };
            logger.Log(LogLevel.Warning, "Hello");

            Assert.IsTrue(File.Exists(filePath));
            Assert.Contains("Hello", File.ReadAllText(filePath));
        }
        finally
        {
            File.Delete(filePath);
        }
    }

}
