using System;
using Moq;
using Xunit;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void Constructor_NullOutput_ThrowsArgumentNullException()
    {
        var serviceMock = new Mock<IJokeService>();

        var exception = Assert.Throws<ArgumentNullException>(
            () => new Jester(null!, serviceMock.Object));

        Assert.Equal("jokeOutput", exception.ParamName);
    }

    [Fact]
    public void Constructor_NullService_ThrowsArgumentNullException()
    {
        var outputMock = new Mock<IJokeOutput>();

        var exception = Assert.Throws<ArgumentNullException>(
            () => new Jester(outputMock.Object, null!));

        Assert.Equal("jokeService", exception.ParamName);
    }

    [Fact]
    public void TellJoke_NormalJoke_OutputsJoke()
    {
        var serviceMock = new Mock<IJokeService>();
        var outputMock = new Mock<IJokeOutput>();

        serviceMock.Setup(s => s.GetJoke())
            .Returns("Why did the chicken cross the road?");

        var jester = new Jester(
            outputMock.Object,
            serviceMock.Object);

        jester.TellJoke();

        serviceMock.Verify(s => s.GetJoke(), Times.Once);
        outputMock.Verify(
            o => o.OutputJoke("Why did the chicken cross the road?"),
            Times.Once);
    }

    [Fact]
    public void TellJoke_ChuckNorrisJoke_SkipsAndGetsAnother()
    {
        var serviceMock = new Mock<IJokeService>();
        var outputMock = new Mock<IJokeOutput>();

        serviceMock.SetupSequence(s => s.GetJoke())
            .Returns("Chuck Norris can divide by zero.")
            .Returns("A normal joke.");

        var jester = new Jester(
            outputMock.Object,
            serviceMock.Object);

        jester.TellJoke();

        serviceMock.Verify(s => s.GetJoke(), Times.Exactly(2));
        outputMock.Verify(
            o => o.OutputJoke("A normal joke."),
            Times.Once);

        outputMock.Verify(
            o => o.OutputJoke(
                It.Is<string>(j => j.Contains("Chuck Norris"))),
            Times.Never);
    }

    [Fact]
    public void TellJoke_MultipleChuckNorrisJokes_SkipsAll()
    {
        var serviceMock = new Mock<IJokeService>();
        var outputMock = new Mock<IJokeOutput>();

        serviceMock.SetupSequence(s => s.GetJoke())
            .Returns("Chuck Norris joke one.")
            .Returns("Chuck Norris joke two.")
            .Returns("A safe joke.");

        var jester = new Jester(
            outputMock.Object,
            serviceMock.Object);

        jester.TellJoke();

        serviceMock.Verify(s => s.GetJoke(), Times.Exactly(3));
        outputMock.Verify(
            o => o.OutputJoke("A safe joke."),
            Times.Once);
    }
}
