using Microsoft.VisualStudio.TestPlatform.Utilities;
using Moq;
using System;
using System.IO;
using Xunit;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void JesterConstructor_ValidParameters_CreatesJesterInstance()
    {
        // Arrange
        Mock<IJokeService> jokeServiceMock = new();
        Mock<IOutputService> outputServiceMock = new();

        // Act
        Jester jester = new(outputServiceMock.Object, jokeServiceMock.Object);

        // Assert
        Assert.NotNull(jester);
    }

    [Fact]
    public void JesterConstructor_OutputServiceIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Mock<IJokeService> jokeServiceMock = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(null!, jokeServiceMock.Object));
    }

    [Fact]
    public void JesterConstructor_JokeServiceIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Mock<IOutputService> outputServiceMock = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(outputServiceMock.Object, null!));
    }

    [Fact]
    public void TellJoke_JokeDoesNotContainChuckNorris_WritesJokeToOutput()
    {
        // Arrange
        string expectedJoke = "Why did the scarecrow win an award? Because he was outstanding in his field!";
        Mock<IJokeService> jokeServiceMock = new();
        jokeServiceMock.Setup(js => js.GetJoke()).Returns(expectedJoke);
        Mock<IOutputService> outputServiceMock = new();
        Jester jester = new(outputServiceMock.Object, jokeServiceMock.Object);

        // Act
        jester.TellJoke();

        // Assert
        outputServiceMock.Verify(os => os.Write(expectedJoke), Times.Once);
    }

    [Fact]
    public void TellJoke_JokeContainsChuckNorris_GetsNewJokeAndWritesToOutput()
    {
        // Arrange
        string chuckNorrisJoke = "Chuck Norris can divide by zero.";
        string validJoke = "Why don't scientists trust atoms? Because they make up everything!";
        Mock<IJokeService> jokeServiceMock = new();
        jokeServiceMock.SetupSequence(js => js.GetJoke())
                       .Returns(chuckNorrisJoke)
                       .Returns(validJoke);
        Mock<IOutputService> outputServiceMock = new();
        Jester jester = new(outputServiceMock.Object, jokeServiceMock.Object);

        // Act
        jester.TellJoke();

        // Assert
        outputServiceMock.Verify(os => os.Write(validJoke), Times.Once);
        outputServiceMock.Verify(os => os.Write(It.Is<string>(j => j.Contains("Chuck Norris"))), Times.Never);
    }

    // Extra Credit Test:
    [Fact]
    public void Jester_TellJoke_PrintsToConsole()
    {
        // Arrange
        using StringWriter stringWriter = new();
        Console.SetOut(stringWriter);

        string expectedJoke = "Why don't programmers like nature? It has too many bugs.";
        IJokeService jokeService = Mock.Of<IJokeService>(js => js.GetJoke() == expectedJoke);
        IOutputService outputService = new ConsoleOutputService();
        Jester jester = new(outputService, jokeService);

        // Act
        jester.TellJoke();

        // Assert
        string consoleOutput = stringWriter.ToString().Trim();
        Assert.Equal(expectedJoke, consoleOutput);
    }



}
