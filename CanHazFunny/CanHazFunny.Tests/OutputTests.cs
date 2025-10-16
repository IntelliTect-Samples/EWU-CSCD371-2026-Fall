using Xunit;
using System;

namespace CanHazFunny.Tests;

public class OutputTests
{
    [Fact]
    public void Output_WriteLine_WritesToConsole()
    {
        // Arrange
        var output = new ConsoleOutput();
        var message = "Hello, World!";
        using var consoleOutput = new System.IO.StringWriter();
        Console.SetOut(consoleOutput);
        // Act
        output.Write(message);
        // Assert
        Assert.Equal(message + Environment.NewLine, consoleOutput.ToString());
    }

    [Fact]
    public void Output_WriteLine_HandlesEmptyString()
    {
        // Arrange
        var output = new ConsoleOutput();
        var message = string.Empty;
        using var consoleOutput = new System.IO.StringWriter();
        Console.SetOut(consoleOutput);
        // Act
        output.Write(message);
        // Assert
        Assert.Equal(Environment.NewLine, consoleOutput.ToString());
    }
}
