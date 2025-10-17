using System;
using System.IO;
using Xunit;
namespace CanHazFunny.Tests;

public class JokeOutputTests
{
    [Fact]
    public void WriteLine_ValidMessage_WritesToConsole()
    {
        // Arrange
        var sw = new StringWriter();
        var originalOutput = Console.Out;

        try
        {
            Console.SetOut(sw);
            JokeOutput jokeOutput = new JokeOutput();
            string testMessage = "This is a test joke.";

            // Act
            jokeOutput.WriteLine(testMessage);

            // Assert
            Assert.Contains(testMessage, sw.ToString());
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }
}