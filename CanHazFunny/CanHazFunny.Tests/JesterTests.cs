using Xunit;
using System;
using Moq;
using System.IO;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void Jester_OutputNull_ArgumentNullException()
    {
        // Arrange
        JokeService jokeService = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(null!, jokeService));
    }

    [Fact]
    public void Jester_JokeServiceNull_ArgumentNullException()
    {
        // Arrange
        ConsoleOutput consoleOutput = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(consoleOutput, null!));
    }

    [Fact]
    public void TellJoke_NonChuckNorrisJoke_PrintsJoke()
    {
        // Arrange
        string testJoke = "test output";
        var mockOutput = new Mock<IOutput>();
        var mockJokeService = new Mock<IJokeService>();
        mockJokeService.SetupSequence(js => js.GetJoke())
            .Returns(testJoke);
        StringWriter sw = new();
        TextWriter original = Console.Out;

        // Act
        try
        {
            Console.SetOut(sw);
            var jester = new Jester(new ConsoleOutput(), mockJokeService.Object);
            jester.TellJoke();
        }
        finally
        {
            Console.SetOut(original);
        }

        // Assert
        Assert.Contains(testJoke, sw.ToString());
    }

    [Fact]
    public void TellJoke_ChuckNorrisJoke_GeneratesNewJoke()
    {
        // Arrange
        var mockOutput = new Mock<IOutput>();
        var mockJokeService = new Mock<IJokeService>();
        mockJokeService.SetupSequence(js => js.GetJoke())
            .Returns("Norris counted to infinity. Twice.")
            .Returns("Chuck Norris can divide by zero.")
            .Returns("A test joke.");
        StringWriter sw = new();
        TextWriter original = Console.Out;

        // Act
        try
        {
            Console.SetOut(sw);
            var jester = new Jester(new ConsoleOutput(), mockJokeService.Object);
            jester.TellJoke();
        }
        finally
        {
            Console.SetOut(original);
        }

        // Assert
        string output = sw.ToString();
        Assert.DoesNotContain("Chuck", output);
        Assert.DoesNotContain("Norris", output);
        Assert.Contains("A test joke.", output);
    }
}
