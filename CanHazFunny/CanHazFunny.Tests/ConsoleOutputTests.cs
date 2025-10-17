using Xunit;
using System;

namespace CanHazFunny.Tests;

public class ConsoleOutputTests
{
    [Fact]
    public void ShowOutput_MessageIsNull_ArgumentNullException()
    { 
        // Arrange
        ConsoleOutput consoleOutput = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => consoleOutput.ShowOutput(null!));
    }

    [Fact]
    public void ShowOutput_MessageIsEmpty_ArgumentException()
    {
        // Arrange
        ConsoleOutput consoleOutput = new();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => consoleOutput.ShowOutput(string.Empty));
    }

    [Fact]
    public void ShowOutput_MessageIsWhitespace_ArgumentException()
    {
        // Arrange
        ConsoleOutput consoleOutput = new();

        // Act & Assert
        Assert.Throws<ArgumentException>(() => consoleOutput.ShowOutput("   "));
    }
}