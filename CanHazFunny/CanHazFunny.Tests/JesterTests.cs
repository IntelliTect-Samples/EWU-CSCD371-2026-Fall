using Xunit;
using Moq;
using System;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void Constructor_CorrectParams_CreatesJester()
    {
        // Arrange
        var jokeService = new Mock<IJokeService>().Object;
        var outputService = new Mock<IOutputService>().Object;
        // Act
        var jester = new Jester(jokeService, outputService);
        // Assert
        Assert.NotNull(jester);
    }

    [Fact]
    public void Constructor_NullParams_ThrowsArgumentNullException()
    {
        // Arrange
        var jokeService = new Mock<IJokeService>().Object;
        var outputService = new Mock<IOutputService>().Object;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(null!, outputService));
        Assert.Throws<ArgumentNullException>(() => new Jester(jokeService, null!));
    }

    [Fact]
    public void TellJoke_NormalJoke_WritesJoke()
    {
        // Arrange
        var jokeServiceMock = new Mock<IJokeService>();
        var outputServiceMock = new Mock<IOutputService>();

        var normalJoke = "Why did the programmer quit his job? Because he didn't get arrays.";
        jokeServiceMock.Setup(x => x.GetJoke()).Returns(normalJoke);

        var jester = new Jester(jokeServiceMock.Object, outputServiceMock.Object);

        // Act
        jester.TellJoke();

        // Assert
        jokeServiceMock.Verify(x => x.GetJoke(), Times.Once);
        outputServiceMock.Verify(x => x.WriteJoke(normalJoke), Times.Once);
    }

    [Fact]
    public void TellJoke_ChuckNorrisJoke_RetriesUntilNormalJoke()
    {
        // Arrange
        var jokeServiceMock = new Mock<IJokeService>();
        var outputServiceMock = new Mock<IOutputService>();

        var chuckNorrisJoke = "Chuck Norris can divide by zero.";
        var normalJoke = "Why do programmers prefer dark mode? Because light attracts bugs.";

        jokeServiceMock.SetupSequence(x => x.GetJoke())
            .Returns(chuckNorrisJoke)
            .Returns(chuckNorrisJoke)
            .Returns(normalJoke);

        var jester = new Jester(jokeServiceMock.Object, outputServiceMock.Object);
        // Act
        jester.TellJoke();
        // Assert
        jokeServiceMock.Verify(x => x.GetJoke(), Times.Exactly(3));
        outputServiceMock.Verify(x => x.WriteJoke(normalJoke), Times.Once);
    }

    [Fact]
    public void TellJoke_OnlyChuckNorrisJokes_AfterMaxRetries_WritesApology()
    {
        // Arrange
        var jokeServiceMock = new Mock<IJokeService>();
        var outputServiceMock = new Mock<IOutputService>();
        var chuckNorrisJoke = "Chuck Norris can divide by zero.";
        var apologyMessage = "Sorry, couldn't find a non-Chuck Norris joke after 50 attempts.";
        jokeServiceMock.Setup(x => x.GetJoke()).Returns(chuckNorrisJoke);
        var jester = new Jester(jokeServiceMock.Object, outputServiceMock.Object);
        // Act
        jester.TellJoke();
        // Assert
        jokeServiceMock.Verify(x => x.GetJoke(), Times.Exactly(51));
        outputServiceMock.Verify(x => x.WriteJoke(apologyMessage), Times.Once);
    }
}
