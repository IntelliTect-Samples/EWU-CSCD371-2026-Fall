using System;
using Moq;
using Xunit;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void Constructor_NullJokeOutput_ThrowsArgumentNullException()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
            () => new Jester(null!, new Mock<IJokeService>().Object));

        Assert.Equal("jokeOutput", ex.ParamName);
    }

    [Fact]
    public void Constructor_NullJokeService_ThrowsArgumentNullException()
    {
        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
            () => new Jester(new Mock<IJokeOutput>().Object, null!));

        Assert.Equal("jokeService", ex.ParamName);
    }

    [Fact]
    public void TellJoke_CleanJoke_WritesItToOutputOnce()
    {
        Mock<IJokeService> service = new();
        service.Setup(s => s.GetJoke()).Returns("A funny joke");
        Mock<IJokeOutput> output = new();

        new Jester(output.Object, service.Object).TellJoke();

        output.Verify(o => o.WriteJoke("A funny joke"), Times.Once);
        service.Verify(s => s.GetJoke(), Times.Once);
    }

    [Theory]
    [InlineData("Chuck Norris can divide by zero")]

    public void TellJoke_ChuckNorrisJoke_SkipsItAndGetsAnother(string badJoke)
    {
        Mock<IJokeService> service = new();
        service.SetupSequence(s => s.GetJoke())
            .Returns(badJoke)
            .Returns("A good joke");
        Mock<IJokeOutput> output = new();

        new Jester(output.Object, service.Object).TellJoke();

        output.Verify(o => o.WriteJoke(badJoke), Times.Never);
        output.Verify(o => o.WriteJoke("A good joke"), Times.Once);
        service.Verify(s => s.GetJoke(), Times.Exactly(2));
    }

    [Fact]
    public void TellJoke_SeveralChuckNorrisJokesInARow_KeepsRetrying()
    {
        Mock<IJokeService> service = new();
        service.SetupSequence(s => s.GetJoke())
            .Returns("Chuck Norris 1")
            .Returns("Chuck Norris 2")
            .Returns("Chuck Norris 3")
            .Returns("Finally, a good one");
        Mock<IJokeOutput> output = new();

        new Jester(output.Object, service.Object).TellJoke();

        output.Verify(o => o.WriteJoke(It.IsAny<string>()), Times.Once);
        output.Verify(o => o.WriteJoke("Finally, a good one"), Times.Once);
    }
}
