using System;
using System.IO;
using Xunit;

namespace CanHazFunny.Tests;

public class ConsoleOutputServiceTests
{
    [Fact]
    public void WriteJoke_WithMessage_WritesToConsole()
    {
        // Arrange
        var service = new ConsoleOutputService();
        var joke = "Why did the programmer quit his job? Because he didn't get arrays.";
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            service.WriteJoke(joke);

            // Assert
            Assert.Equal(joke + Environment.NewLine, stringWriter.ToString());
        }
        finally
        {
            // Cleanup - restore console output
            Console.SetOut(Console.Out);
        }
    }

    [Fact]
    public void WriteJoke_WithEmptyString_WritesEmptyLine()
    {
        // Arrange
        var service = new ConsoleOutputService();
        var stringWriter = new StringWriter();
        Console.SetOut(stringWriter);

        try
        {
            // Act
            service.WriteJoke(string.Empty);

            // Assert
            Assert.Equal(Environment.NewLine, stringWriter.ToString());
        }
        finally
        {
            // Cleanup - restore console output
            Console.SetOut(Console.Out);
        }
    }
}
