using Xunit;
using Moq;
using System;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void Constructor_NullOutput_ThrowsArgumentNullException()
    {
        // Arrange
        var jokeServiceMock = new Mock<IJokeService>().Object;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(null!, jokeServiceMock));
    }

    [Fact]
    public void Constructor_NullJokeService_ThrowsArgumentNullException()
    {
        // Arrange
        var outputMock = new Mock<IOutput>().Object;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(outputMock, null!));
    }

    [Fact]
    public void TellJoke_ValidJoke_WritesJokeToOutput()
    {
        // Arrange
        var outputMock = new Mock<IOutput>();
        var jokeServiceMock = new Mock<IJokeService>();
        jokeServiceMock.Setup(js => js.GetJoke()).Returns("Why do programmers prefer dark mode? Because light attracts bugs!");
        var jester = new Jester(outputMock.Object, jokeServiceMock.Object);
        // Act
        jester.TellJoke();
        // Assert
        outputMock.Verify(o => o.WriteLine(It.Is<string>(s => s.Contains("programmers"))), Times.Once);
        jokeServiceMock.Verify(js => js.GetJoke(), Times.Once);
    }

    [Fact]
    public void TellJoke_SkipsChuckNorrisJokes_WritesFirstValidJoke()
    {
        // Arrange
        var outputMock = new Mock<IOutput>();
        var jokeServiceMock = new Mock<IJokeService>();
        var callCount = 0;
        jokeServiceMock.Setup(js => js.GetJoke()).Returns(() =>
        {
            callCount++;
            return callCount < 3 ? "Chuck Norris can slam a revolving door." : "Why did the scarecrow win an award? Because he was outstanding in his field!";
        });
        var jester = new Jester(outputMock.Object, jokeServiceMock.Object);
        // Act
        jester.TellJoke();
        // Assert
        outputMock.Verify(o => o.WriteLine(It.Is<string>(s => s.Contains("scarecrow"))), Times.Once);
        jokeServiceMock.Verify(js => js.GetJoke(), Times.Exactly(3));
    }


    [Fact]
    public void TellJoke_ChuckNorrisJoke_RetriesAndWritesFailureMessage()
    {
        // Arrange
        var outputMock = new Mock<IOutput>();
        var jokeServiceMock = new Mock<IJokeService>();
        jokeServiceMock.Setup(js => js.GetJoke()).Returns("Chuck Norris can divide by zero.");
        var jester = new Jester(outputMock.Object, jokeServiceMock.Object);
        // Act
        jester.TellJoke();
        // Assert
        outputMock.Verify(o => o.WriteLine("Failed to find an acceptable joke. Try again!"), Times.Once);
        jokeServiceMock.Verify(js => js.GetJoke(), Times.Exactly(10));
    }

    [Fact]
    public void TellJoke_SkipsLowercaseChuckNorrisJokes_WritesFirstValidJoke()
    {
        // Arrange
        var outputMock = new Mock<IOutput>();
        var jokeServiceMock = new Mock<IJokeService>();
        var callCount = 0;
        jokeServiceMock.Setup(js => js.GetJoke()).Returns(() =>
        {
            callCount++;
            return callCount < 3 ? "chuck norris can slam a revolving door." : "Why did the scarecrow win an award? Because he was outstanding in his field!";
        });
        var jester = new Jester(outputMock.Object, jokeServiceMock.Object);
        // Act
        jester.TellJoke();
        // Assert
        outputMock.Verify(o => o.WriteLine(It.Is<string>(s => s.Contains("scarecrow"))), Times.Once);
        jokeServiceMock.Verify(js => js.GetJoke(), Times.Exactly(3));
    }

    [Fact]
    public void TellJoke_WhenServiceThrowsException_BubblesException()
    {
        var outputMock = new Mock<IOutput>();
        var jokeServiceMock = new Mock<IJokeService>();
        jokeServiceMock.Setup(js => js.GetJoke()).Throws(new InvalidOperationException("Service error"));
        
        var jester = new Jester(outputMock.Object, jokeServiceMock.Object);
        Assert.Throws<InvalidOperationException>(() => jester.TellJoke());
    }
}
