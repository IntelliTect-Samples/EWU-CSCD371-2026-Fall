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
        string loggerFilePath = "Log_WritesToFile_Success.txt";
        logFactory.ConfigureFileLogger(loggerFilePath);
        FileLogger logger = logFactory.CreateLogger<FileLogger>(nameof(FileLoggerTests))!;
        string testString = "TEST";
        // Act
        File.WriteAllText(loggerFilePath, string.Empty);
        logger.Log(LogLevel.Debug, testString);
        // Assert
        try
        {
            string[] fileContent = File.ReadAllLines(loggerFilePath);
            //Test to see that the end of the string is the same.
            Assert.AreEqual(testString, fileContent[0].Substring(fileContent[0].Length - testString.Length));
        }
        catch (IOException e)
        {
            Console.WriteLine($"Error reading file: {e.Message}");
        }
    }
    [TestMethod]
    public void Log_WritesMultipleLinesToFile_Success()
    {
        // Arrange
        LogFactory logFactory = new();
        string loggerFilePath = "Log_WritesMultipleLinesToFile_Success.txt";
        logFactory.ConfigureFileLogger(loggerFilePath);
        FileLogger logger = logFactory.CreateLogger<FileLogger>(nameof(FileLoggerTests))!;
        string firstTestString = "line 1";
        string secondTestString = "line 2";
        string thirdTestString = "line 3";

        // Act
        File.WriteAllText(loggerFilePath, string.Empty);
        logger.Log(LogLevel.Debug, firstTestString);
        logger.Log(LogLevel.Debug, secondTestString);
        logger.Log(LogLevel.Debug, thirdTestString);
        // Assert
        try
        {
            string[] fileContent = File.ReadAllLines(loggerFilePath);
            
            //Remove the debug junk from the string. Focus only on the ending message.
            Assert.AreEqual(firstTestString, fileContent[0].Substring(fileContent[0].Length - firstTestString.Length));
            Assert.AreEqual(secondTestString, fileContent[1].Substring(fileContent[1].Length - secondTestString.Length));
            Assert.AreEqual(thirdTestString, fileContent[2].Substring(fileContent[2].Length - thirdTestString.Length));

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
        FileLogger logger = logFactory.CreateLogger<FileLogger>(nameof(FileLoggerTests))!;
        // Assert
        Assert.IsNull(logger);
    }
    [TestMethod]
    public void CreateLogger_CreatedWithLogFactory_IsValid()
    {
        // Arrange
        LogFactory logFactory = new();
        logFactory.ConfigureFileLogger("CreateLogger_CreatedWithLogFactory_IsValid.txt");
        // Act
        FileLogger logger = logFactory.CreateLogger<FileLogger>(nameof(FileLoggerTests))!;
        // Assert
        Assert.IsNotNull(logger);
    }
}
