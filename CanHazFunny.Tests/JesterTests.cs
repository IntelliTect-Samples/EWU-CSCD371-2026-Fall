using System;
using System.IO;
using System.Threading.Tasks;

using CanHazFunny.Core;
using CanHazFunny.Output;
using CanHazFunny.Services;

using Moq;

using Xunit;

namespace CanHazFunny.Tests;

public sealed class JesterTests
{
    [Fact]
    public async Task TellJokeAsync_SkipsChuckNorrisJokes()
    {
        var jokeService = new Mock<IJokeBuilder>();
        var output = new Mock<IJokeOutputStringer>();

        jokeService.SetupSequence(s => s.GetJokeAsync())
            .ReturnsAsync("Chuck Norris counted to infinity. Twice.")
            .ReturnsAsync("A SQL query walks into a bar...");

        var jester = new Jester(jokeService.Object, output.Object);

        await jester.TellJoke();

        output.Verify(o => o.Write("A SQL query walks into a bar..."), Times.Once);
    }

    [Fact]
    public void Constructor_ThrowsOnNullDependencies()
    {
        var output = new Mock<IJokeOutputStringer>();

        Assert.Throws<ArgumentNullException>(() => new Jester(null!, output.Object));
        Assert.Throws<ArgumentNullException>(() => new Jester(new Mock<IJokeBuilder>().Object, null!));
    }
    [Fact]
public void Constructor_ThrowsOnNullJokeBuilder()
{
    var output = new Mock<IJokeOutputStringer>();

    Assert.Throws<ArgumentNullException>(() =>
        new Jester(null!, output.Object));
}

// Constructor must throw on null IJokeOutputStringer
[Fact]
public void Constructor_ThrowsOnNullOutput()
{
    var jokeService = new Mock<IJokeBuilder>();

    Assert.Throws<ArgumentNullException>(() =>
        new Jester(jokeService.Object, null!));
}

// Jester must skip Chuck Norris jokes
[Fact]
public async Task TellJoke_SkipsChuckNorrisJokes()
{
    var jokeService = new Mock<IJokeBuilder>();
    var output = new Mock<IJokeOutputStringer>();

    jokeService.SetupSequence(s => s.GetJokeAsync())
        .ReturnsAsync("Chuck Norris counted to infinity. Twice.")
        .ReturnsAsync("A SQL query walks into a bar...");

    var jester = new Jester(jokeService.Object, output.Object);

    await jester.TellJoke();

    output.Verify(o => o.Write("A SQL query walks into a bar..."), Times.Once);
}

    // 4. Jester must skip MULTIPLE Chuck Norris jokes in a row
    [Fact]
    public async Task TellJokeAsync_SkipsMultipleChuckNorrisJokes()
    {
        var jokeService = new Mock<IJokeBuilder>();
        var output = new Mock<IJokeOutputStringer>();

        jokeService.SetupSequence(s => s.GetJokeAsync())
            .ReturnsAsync("Chuck Norris counted to infinity. Twice.")
            .ReturnsAsync("CHUCK NORRIS doesn’t read books.")
            .ReturnsAsync("chuck norris can divide by zero.")
            .ReturnsAsync("A SQL query walks into a bar...");

        var jester = new Jester(jokeService.Object, output.Object);

        await jester.TellJoke();

        output.Verify(o => o.Write("A SQL query walks into a bar..."), Times.Once);
    }

    // 5. Jester must call GetJokeAsync until a non-Chuck joke appears
    [Fact]
    public async Task TellJoke_CallsGetJokeUntilNonChuckJoke()
    {
        var jokeService = new Mock<IJokeBuilder>();
        var output = new Mock<IJokeOutputStringer>();

        jokeService.SetupSequence(s => s.GetJokeAsync())
            .ReturnsAsync("Chuck Norris counted to infinity. Twice.")
            .ReturnsAsync("A SQL query walks into a bar...");

        var jester = new Jester(jokeService.Object, output.Object);

        await jester.TellJoke();

        jokeService.Verify(s => s.GetJokeAsync(), Times.Exactly(2));
    }

    // 6. Jester must write exactly one joke
    [Fact]
    public async Task TellJoke_WritesExactlyOneJoke()
    {
        var jokeService = new Mock<IJokeBuilder>();
        var output = new Mock<IJokeOutputStringer>();

        jokeService.SetupSequence(s => s.GetJokeAsync())
            .ReturnsAsync("Chuck Norris counted to infinity. Twice.")
            .ReturnsAsync("A SQL query walks into a bar...");

        var jester = new Jester(jokeService.Object, output.Object);

        await jester.TellJoke();

        output.Verify(o => o.Write(It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task TellJoke_WritesCorrectJoke()
    {
        var jokeService = new Mock<IJokeBuilder>();
        var output = new Mock<IJokeOutputStringer>();

        jokeService.SetupSequence(s => s.GetJokeAsync())
            .ReturnsAsync("Chuck Norris counted to infinity. Twice.")
            .ReturnsAsync("A SQL query walks into a bar...");

        var jester = new Jester(jokeService.Object, output.Object);

        await jester.TellJoke();

        output.Verify(o => o.Write("A SQL query walks into a bar..."), Times.Once);
    }
    [Fact]
    public void ConsoleJokingOutput_WritesToConsole()
    {
        var output = new ConsoleJokingOutput();

        using var sw = new StringWriter();
        Console.SetOut(sw);

        output.Write("Hello!");

        Assert.Equal("Hello!" + Environment.NewLine, sw.ToString());
    }
}
