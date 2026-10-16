using System;
using Moq;
using Xunit;

namespace CanHazFunny.Tests;

public class JesterTests
{
    private readonly Mock<IJokeOutput> _output = new();
    private readonly Mock<IJokeService> _jokeService = new();

    [Fact]
    public void ConstructorThrowsWhenOutputIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
            () => { _ = new Jester(null!, _jokeService.Object); });

        Assert.Equal("jokeOutput", exception.ParamName);
    }

    [Fact]
    public void ConstructorThrowsWhenJokeServiceIsNull()
    {
        ArgumentNullException exception = Assert.Throws<ArgumentNullException>(
            () => { _ = new Jester(_output.Object, null!); });

        Assert.Equal("jokeService", exception.ParamName);
    }

    [Fact]
    public void TellJokeWritesTheJokeFromTheService()
    {
        _jokeService.Setup(s => s.GetJoke()).Returns("A funny joke");
        Jester jester = new(_output.Object, _jokeService.Object);

        jester.TellJoke();

        _output.Verify(o => o.Write("A funny joke"), Times.Once);
        _jokeService.Verify(s => s.GetJoke(), Times.Once);
    }

    [Fact]
    public void TellJokeSkipsJokesContainingChuckNorris()
    {
        _jokeService.SetupSequence(s => s.GetJoke())
            .Returns("Chuck Norris can divide by zero.")
            .Returns("A clean joke");
        Jester jester = new(_output.Object, _jokeService.Object);

        jester.TellJoke();

        _jokeService.Verify(s => s.GetJoke(), Times.Exactly(2));
        _output.Verify(o => o.Write("A clean joke"), Times.Once);
        _output.Verify(o => o.Write(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public void TellJokeKeepsSkippingUntilAJokeWithoutChuckNorrisIsFound()
    {
        _jokeService.SetupSequence(s => s.GetJoke())
            .Returns("Chuck Norris joke one")
            .Returns("Another Chuck Norris joke")
            .Returns("Yet another Chuck Norris joke")
            .Returns("Finally a good one");
        Jester jester = new(_output.Object, _jokeService.Object);

        jester.TellJoke();

        _jokeService.Verify(s => s.GetJoke(), Times.Exactly(4));
        _output.Verify(o => o.Write("Finally a good one"), Times.Once);
    }

    [Fact]
    public void TellJokeSkipsChuckNorrisJokesRegardlessOfCase()
    {
        _jokeService.SetupSequence(s => s.GetJoke())
            .Returns("why is CHUCK NORRIS so tough?")
            .Returns("A clean joke");
        Jester jester = new(_output.Object, _jokeService.Object);

        jester.TellJoke();

        _output.Verify(o => o.Write("A clean joke"), Times.Once);
    }

    [Fact]
    public void TellJokeNeverWritesAChuckNorrisJoke()
    {
        _jokeService.SetupSequence(s => s.GetJoke())
            .Returns("Chuck Norris joke")
            .Returns("Safe joke");
        Jester jester = new(_output.Object, _jokeService.Object);

        jester.TellJoke();

        _output.Verify(
            o => o.Write(It.Is<string>(joke => joke.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase))),
            Times.Never);
    }
}