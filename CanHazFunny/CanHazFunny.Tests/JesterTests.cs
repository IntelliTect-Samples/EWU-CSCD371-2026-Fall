using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using Xunit;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void Constructor_WithValidDependencies_SetsProperties()
    {
        // Arrange
        Mock<IOutput> outputMock = new();
        Mock<IJokeService> serviceMock = new();

        // Act
        Jester jester = new(outputMock.Object, serviceMock.Object);

        // Assert
        Assert.Same(outputMock.Object, jester.JokeOutput);
        Assert.Same(serviceMock.Object, jester.JokeService);
    }

    [Fact]
    public void Constructor_WithNullOutput_ThrowsArgumentNullException()
    {
        // Arrange
        Mock<IJokeService> serviceMock = new();

        // Act
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new Jester(null!, serviceMock.Object));

        // Assert
        Assert.Equal("jokeOutput", ex.ParamName);
    }

    [Fact]
    public void Constructor_WithNullService_ThrowsArgumentNullException()
    {
        // Arrange
        Mock<IOutput> outputMock = new();

        // Act / Assert
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(() => new Jester(outputMock.Object, null!));
    }

    [Fact]
    public void TellJoke_CallsGetJoke_And_PrintJoke_WithReturnedValue()
    {
        // Arrange
        string expectedJoke = "Why did the developer go broke? Because he used up all his cache.";

        Mock<IOutput> outputMock = new();
        Mock<IJokeService> serviceMock = new();
        serviceMock.Setup(s => s.GetJoke()).Returns(expectedJoke);

        Jester jester = new(outputMock.Object, serviceMock.Object);

        // Act
        jester.TellJoke();

        // Assert
        serviceMock.Verify(s => s.GetJoke(), Times.Once);
        outputMock.Verify(o => o.Write(expectedJoke), Times.Once);
    }

    [Fact]
    public void TellJoke_WhenServiceReturnsEmptyString_StillCallsPrintJoke()
    {
        // Arrange
        Mock<IOutput> outputMock = new();
        Mock<IJokeService> serviceMock = new();
        serviceMock.Setup(s => s.GetJoke()).Returns(string.Empty);

        Jester jester = new(outputMock.Object, serviceMock.Object);

        // Act
        jester.TellJoke();

        // Assert
        outputMock.Verify(o => o.Write(string.Empty), Times.Once);
    }

    [Fact]
    public void TellJoke_WhenServiceThrows_PropagatesException()
    {
        // Arrange
        Mock<IOutput> outputMock = new();
        Mock<IJokeService> serviceMock = new();
        serviceMock.Setup(s => s.GetJoke()).Throws(new InvalidOperationException("bad joke source"));

        Jester jester = new(outputMock.Object, serviceMock.Object);

        // Act + Assert
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => jester.TellJoke());
        Assert.Equal("bad joke source", ex.Message);
    }

    [Fact]
    public void TellJoke_WhenOutputThrows_PropagatesException()
    {
        // Arrange
        Mock<IOutput> outputMock = new();
        Mock<IJokeService> serviceMock = new();
        serviceMock.Setup(s => s.GetJoke()).Returns("Some joke");
        outputMock.Setup(o => o.Write(It.IsAny<string>())).Throws(new IOException("printer jam"));

        Jester jester = new(outputMock.Object, serviceMock.Object);

        // Act + Assert
        IOException ex = Assert.Throws<IOException>(() => jester.TellJoke());
        Assert.Equal("printer jam", ex.Message);
    }

    [Fact]
    public void TellJoke_WhenServiceReturnsChuckNorrisJokes_FiltersUntilCleanJoke()
    {
        // Arrange
        Mock<IOutput> outputMock = new();
        Mock<IJokeService> serviceMock = new();

        Queue<string> jokes = new (
        [
            "Chuck Norris counted to infinity. Twice.",
            "The dinosaurs looked at Chuck Norris the wrong way once.",
            "A clean, safe, corporate-approved joke."
        ]);

        serviceMock.Setup(s => s.GetJoke()).Returns(() => jokes.Dequeue());

        Jester jester = new(outputMock.Object, serviceMock.Object);

        // Act
        jester.TellJoke();

        // Assert
        serviceMock.Verify(s => s.GetJoke(), Times.Exactly(3));
        outputMock.Verify(o => o.Write("A clean, safe, corporate-approved joke."), Times.Once);
    }

    [Theory]
    [InlineData("Chuck Norris once roundhouse kicked a server into uptime.")]
    [InlineData("I heard chuck norris debugs in production.")]
    [InlineData("CHUCK NORRIS DOES NOT NEED UNIT TESTS.")]
    public void TellJoke_FiltersCaseInsensitively_CatchesLoop(string badJoke)
    {
        // Arrange
        Mock<IOutput> outputMock = new();
        Mock<IJokeService> serviceMock = new();
        bool isCalled = false;

        serviceMock.Setup(s => s.GetJoke()).Returns(() =>
        {
            isCalled = true;
            if (isCalled) throw new InvalidOperationException("infinite chuck norris loop");
            return badJoke;
        });

        Jester jester = new(outputMock.Object, serviceMock.Object);

        // Act + Assert
        InvalidOperationException ex = Assert.Throws<InvalidOperationException>(() => jester.TellJoke());
    }
}
