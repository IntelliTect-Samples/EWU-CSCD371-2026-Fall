using Xunit;
using Moq;
using System;
using System.IO;

namespace CanHazFunny.Tests;


public class JesterTests
{
    private readonly Mock<InterfaceJokeService> _jokeService = new();
    private readonly Mock<InterfaceConsoleOutput> _output = new();
    private readonly Jester _sut;

    public JesterTests()
    {
        _sut = new Jester(_jokeService.Object, _output.Object);
    }

    // ---------- Constructor ----------

    [Fact]
    public void Constructor_NullJokeService_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => new Jester(null!, _output.Object));

        Assert.Equal("jokeService", ex.ParamName);
    }

    [Fact]
    public void Constructor_NullOutput_ThrowsArgumentNullException()
    {
        var ex = Assert.Throws<ArgumentNullException>(
            () => new Jester(_jokeService.Object, null!));

        Assert.Equal("output", ex.ParamName);
    }

    [Fact]
    public void Constructor_ValidDependencies_DoesNotThrow()
    {
        var jester = new Jester(_jokeService.Object, _output.Object);

        Assert.NotNull(jester);
    }

    // ---------- TellJoke: happy path ----------

    [Fact]
    public void TellJoke_CleanJoke_WritesJokeExactlyOnce()
    {
        _jokeService.Setup(s => s.GetJoke()).Returns("Why did the chicken cross the road?");

        _sut.TellJoke();

        _jokeService.Verify(s => s.GetJoke(), Times.Once);
        _output.Verify(o => o.WriteJoke("Why did the chicken cross the road?"), Times.Once);
    }

    // ---------- TellJoke: Chuck Norris filtering ----------

    [Fact]
    public void TellJoke_FirstJokeContainsChuckNorris_FetchesAnotherAndWritesThat()
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
    public void TellJoke_MultipleChuckNorrisJokesInARow_KeepsRetryingUntilClean()
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
    public void TellJoke_ChuckNorrisInAnyCaseOrPosition_IsDiscarded(string badJoke)
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
    public void TellJoke_PartialMatchOrEmptyJoke_IsNotDiscarded(string okJoke)
    {
        _jokeService.Setup(s => s.GetJoke()).Returns(okJoke);

        _sut.TellJoke();

        _jokeService.Verify(s => s.GetJoke(), Times.Once);
        _output.Verify(o => o.WriteJoke(okJoke), Times.Once);
    }

    // ---------- TellJoke: dependency failures bubble up ----------

    [Fact]
    public void TellJoke_JokeServiceThrows_ExceptionPropagatesAndNothingIsWritten()
    {
        _jokeService.Setup(s => s.GetJoke()).Throws<InvalidOperationException>();

        Assert.Throws<InvalidOperationException>(() => _sut.TellJoke());

        _output.Verify(o => o.WriteJoke(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public void TellJoke_OutputThrows_ExceptionPropagates()
    {
        _jokeService.Setup(s => s.GetJoke()).Returns("A joke");
        _output.Setup(o => o.WriteJoke(It.IsAny<string>())).Throws<IOException>();

        Assert.Throws<IOException>(() => _sut.TellJoke());
    }
}