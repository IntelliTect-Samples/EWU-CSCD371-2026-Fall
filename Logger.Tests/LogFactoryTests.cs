using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{

    [TestMethod]
    public void CreateLogger_WhenNotConfigured_ReturnsNull()
    {
        //Arrange
        var factory = new LogFactory();
        //Act
        var logger = factory.CreateLogger(nameof(LogFactoryTests));
        //Assert
        Assert.IsNull(logger);
    }

    [TestMethod]
    public void CreateLogger_WhenConfigured_ReturnsFileLogger()
    {
        //Arrange
        var factory = new LogFactory();
        string path = "goodpath";
        factory.ConfigureFileLogger(path);
        //Act
        var logger = factory.CreateLogger(nameof(LogFactoryTests));
        //Assert
        Assert.IsNotNull(logger);
        Assert.IsInstanceOfType<FileLogger>(logger);
        Assert.AreEqual((logger as FileLogger)?.GetFilePath(), path);
    }

    [TestMethod]
    public void CreateLogger_WhenConfigured_SetsClassNameFromCaller()
    {
        //Arrange
        var factory = new LogFactory();
        var path = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        factory.ConfigureFileLogger(path);
        //Act
        var logger = factory.CreateLogger(nameof(LogFactoryTests));
        //Assert
        Assert.IsNotNull(logger);
        Assert.AreEqual(nameof(LogFactoryTests), logger!.ClassName);
    }


    [TestMethod]
    public void ConfigureFileLogger_WhenPathIsNullOrWhitespace_ThrowsArgumentException()
    {
        //Arrange
        var factory = new LogFactory();
        //Act & Assert
        Assert.ThrowsExactly<ArgumentException>(() => factory.ConfigureFileLogger(""));
        Assert.ThrowsExactly<ArgumentException>(() => factory.ConfigureFileLogger("   "));
    }

    [TestMethod]
    public void ConfigureFileLogger_WhenPathIsValid_CreatesLoggerSuccessfully()
    {
        //Arrange
        var factory = new LogFactory();
        string path = "goodpath";
        //Act
        factory.ConfigureFileLogger(path);
        var logger = factory.CreateLogger(nameof(LogFactoryTests));
        //Assert
        Assert.IsNotNull(logger);
        Assert.IsInstanceOfType<FileLogger>(logger);
        Assert.AreEqual((logger as FileLogger)?.GetFilePath(), path);
    }
}
