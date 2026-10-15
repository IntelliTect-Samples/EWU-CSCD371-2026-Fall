using Xunit;
using Moq;
using System;
using System.IO;

namespace CanHazFunny.Tests;


public class JesterTests
{
    private readonly Mock<IJokeService> _jokeService = new();
    private readonly Mock<IConsoleOutput> _output = new();
    private readonly Jester _sut;

    public JesterTests()
    {
        _sut = new Jester(_jokeService.Object, _output.Object);
    }

    // ---------- Constructor ----------

    [Fact]
    public void ConstructorNullJokeServiceThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => new Jester(null!, _output.Object));

        Assert.Equal("jokeService", ex.ParamName);
    }

    [Fact]
    public void ConstructorNullOutputThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => new Jester(_jokeService.Object, null!));

        Assert.Equal("output", ex.ParamName);
    }

    [Fact]
    public void ConstructorValidDependenciesDoesNotThrow()
    {
        var jester = new Jester(_jokeService.Object, _output.Object);

        Assert.NotNull(jester);
    }

    // ---------- TellJoke: happy path ----------

    [Theory]
    [InlineData("Why did the chicken cross the road?")]
    public void TellJokeCleanJokeWritesJokeExactlyOnce(string cleanJoke)
    {
        _jokeService.Setup(s => s.GetJoke()).Returns(cleanJoke);

        _sut.TellJoke();

        _jokeService.Verify(s => s.GetJoke(), Times.Once);
        _output.Verify(o => o.WriteJoke(cleanJoke), Times.Once);
    }

    // ---------- TellJoke: Chuck Norris filtering ----------

    [Fact]
    public void TellJokeFirstJokeContainsChuckNorrisFetchesAnotherAndWritesThat()
    {
        _jokeService.SetupSequence(s => s.GetJoke())
            .Returns("Chuck Norris can divide by zero.")
            .Returns("A clean joke.");

        _sut.TellJoke();

        _jokeService.Verify(s => s.GetJoke(), Times.Exactly(2));
        _output.Verify(o => o.WriteJoke("A clean joke."), Times.Once);
        _output.Verify(o => o.WriteJoke(It.Is<string>(j => j.Contains("Chuck Norris"))), Times.Never);
    }

    [Fact]
    public void TellJokeMultipleChuckNorrisJokesInARowKeepsRetryingUntilClean()
    {
        _jokeService.SetupSequence(s => s.GetJoke())
            .Returns("Chuck Norris joke #1")
            .Returns("Chuck Norris joke #2")
            .Returns("Chuck Norris joke #3")
            .Returns("Finally, a clean one.");

        _sut.TellJoke();

        _jokeService.Verify(s => s.GetJoke(), Times.Exactly(4));
        _output.Verify(o => o.WriteJoke(It.IsAny<string>()), Times.Once);
        _output.Verify(o => o.WriteJoke("Finally, a clean one."), Times.Once);
    }

    [Theory]
    [InlineData("chuck norris")]
    [InlineData("CHUCK NORRIS")]
    [InlineData("Chuck Norris")]
    [InlineData("cHuCk NoRrIs")]
    [InlineData("Did you hear what Chuck Norris did today?")]
    [InlineData("The legend of chuck norris lives on")]
    public void TellJokeChuckNorrisInAnyCaseOrPositionIsDiscarded(string badJoke)
    {
        _jokeService.SetupSequence(s => s.GetJoke())
            .Returns(badJoke)
            .Returns("Safe joke");

        _sut.TellJoke();

        _output.Verify(o => o.WriteJoke(badJoke), Times.Never);
        _output.Verify(o => o.WriteJoke("Safe joke"), Times.Once);
    }

    [Theory]
    [InlineData("Chuck")]
    [InlineData("Norris")]
    [InlineData("Chuck Berry sings, Norris listens")]
    [InlineData("")]
    public void TellJokePartialMatchOrEmptyJokeIsNotDiscarded(string okJoke)
    {
        _jokeService.Setup(s => s.GetJoke()).Returns(okJoke);

        _sut.TellJoke();

        _jokeService.Verify(s => s.GetJoke(), Times.Once);
        _output.Verify(o => o.WriteJoke(okJoke), Times.Once);
    }

    // ---------- TellJoke: dependency failures bubble up ----------

    [Fact]
    public void TellJokeJokeServiceThrowsExceptionPropagatesAndNothingIsWritten()
    {
        _jokeService.Setup(s => s.GetJoke()).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => _sut.TellJoke());

        _output.Verify(o => o.WriteJoke(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void TellJokeOutputThrowsExceptionPropagates()
    {
        _jokeService.Setup(s => s.GetJoke()).Returns("A joke");
        _output.Setup(o => o.WriteJoke(It.IsAny<string>())).Throws<IOException>();

        Assert.Throws<IOException>(() => _sut.TellJoke());
    }
}