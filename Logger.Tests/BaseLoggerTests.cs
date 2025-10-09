using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Logger.Tests;

[TestClass]
public class BaseLoggerTests
{
    [TestMethod]
    public void BaseLogger_WhenCalled_SetsClassNameToTypeName()
    {
        // Arrange
        var logger = new TestLogger();
        // Act
        var className = logger.ClassName;
        // Assert
        Assert.AreEqual(nameof(TestLogger), className);
    }
}
