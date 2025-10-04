using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class FileLoggerTests
{
    [TestMethod]
    public void Log_WritesToFile_Success()
    {
        // Arrange
        LogFactory logFactory = new();
        string loggerFilePath = "out.txt";
        logFactory.ConfigureFileLogger(loggerFilePath);
        FileLogger logger = logFactory.CreateLogger<FileLogger>("MyTestApp")!;
        string testString = "TEST";
        // Act
        logger.Log(LogLevel.Debug, testString);
        // Assert
        try
        {
            string fileContent = File.ReadAllText(loggerFilePath);
            //Test to see that the end of the string is the same.
            Assert.AreEqual(testString, fileContent.Substring(fileContent.Length - testString.Length));
        }
        catch (IOException e)
        {
            Console.WriteLine($"Error reading file: {e.Message}");
        }
    }
    [TestMethod]
    public void CreateLogger_CreatedWithoutLogFactory_IsNull()
    {
        // Arrange
        LogFactory logFactory = new();
        // Act
        FileLogger logger = logFactory.CreateLogger<FileLogger>("MyTestApp")!;
        // Assert
        Assert.IsNull(logger);
    }
    [TestMethod]
    public void CreateLogger_CreatedWithLogFactory_IsValid()
    {
        // Arrange
        LogFactory logFactory = new();
        logFactory.ConfigureFileLogger("fakeFile.txt");
        // Act
        FileLogger logger = logFactory.CreateLogger<FileLogger>("MyTestApp")!;
        // Assert
        Assert.IsNotNull(logger);
    }
}
