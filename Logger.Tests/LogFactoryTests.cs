using System;
using System.IO;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Logger.Tests;

[TestClass]
public class LogFactoryTests
{
    [TestMethod]
    public void LogFactory_ConsoleCreation_Successful()
    {
        // Arrange
        LogFactory logFactory = new();
        // Act
        //ConsoleLogger logger = logFactory.CreateLogger<ConsoleLogger>("MyApp")!;
        // Assert
        //Assert.IsNotNull(logger);
    }
}
