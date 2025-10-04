using System;
using System.Reflection;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void LogFactory_BaseLogger_ThrowsNotImplementedException()
    {
        // Arrange
        LogFactory logFactory = new();
        // Act
        TargetInvocationException ex = Assert.Throws<TargetInvocationException>(() => logFactory.CreateLogger<BaseLogger>(nameof(LogFactoryTests)));
        // Assert
        //The exception gets wrapped and has to be unwinded. That is why a second assert exists within this test.
        Assert.IsInstanceOfType(ex.InnerException, typeof(NotImplementedException));
    }
    [TestMethod]
    public void LogFactory_ChangeFileOutput_Successful()
    {
        // Arrange
        LogFactory logFactory = new();
        // Act
        logFactory.ConfigureFileLogger("out.txt");
        FileLogger logger = logFactory.CreateLogger<FileLogger>(nameof(LogFactoryTests))!;
        logFactory.ConfigureFileLogger("betterOut.txt");
        FileLogger logger2 = logFactory.CreateLogger<FileLogger>(nameof(LogFactoryTests))!;

        // Assert
        Assert.AreNotEqual(logger.Path, logger2.Path);
    }
    [TestMethod]
    public void LogFactory_FileOutputUnique_Successful()
    {
        // Arrange
        LogFactory logFactory = new();
        LogFactory logFactory2 = new();
        // Act
        logFactory.ConfigureFileLogger("out.txt");
        logFactory2.ConfigureFileLogger("betterOut.txt");

        FileLogger logger = logFactory.CreateLogger<FileLogger>(nameof(LogFactoryTests))!;
        FileLogger logger2 = logFactory2.CreateLogger<FileLogger>(nameof(LogFactoryTests))!;

        // Assert
        Assert.AreNotEqual(logger.Path, logger2.Path);
    }
}
