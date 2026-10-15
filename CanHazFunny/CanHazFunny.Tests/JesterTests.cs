using Moq;
using System;
using System.IO;
using Xunit;
namespace CanHazFunny.Tests;

public class JesterTests
{

    [Fact]
    public void TellJokeProducesJoke()
    {
        string joke = "This is a joke";

        Mock<IJokeService> jokeService = new();
        Mock<IJokeOutput> jokeOutput = new();

        jokeService.Setup(s => s.GetJoke()).Returns(joke);
        Jester jester = new(jokeService.Object, jokeOutput.Object);

        jester.TellJoke();

        jokeOutput.Verify(o => o.OutputJoke(joke), Times.Once);
    }

    [Fact]
    public void JokeServiceTest()
    {
        JokeService jokeService = new();
        string joke = jokeService.GetJoke();

        Assert.NotEmpty(joke);
    }

    [Fact]
    public void TellJokeExcludesChuckNorris()
    {

        Mock<IJokeService> jokeService = new();
        Mock<IJokeOutput> jokeOutput = new();

        jokeService.SetupSequence(s => s.GetJoke())
            .Returns("This is a Chuck Norris joke")
            .Returns("this is a regular joke");

        Jester jester = new(jokeService.Object, jokeOutput.Object);
        jester.TellJoke();

        jokeService.Verify(s => s.GetJoke(), Times.Exactly(2));
        jokeOutput.Verify(o => o.OutputJoke(It.Is<string>(s => s.Contains("Chuck Norris"))), Times.Never);
    }

    [Fact]
    public void ConstructorNullJokeServiceThrows()
    {
        JokeService jokeService = null!;
        Mock<IJokeOutput> jokeOutput = new();

        var exception = Record.Exception(() => new Jester(jokeService, jokeOutput.Object));

        var argNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(nameof(jokeService), argNullException.ParamName);
    }

    [Fact]
    public void ConstructorNullJokeOutputThrows()
    {
        Mock<IJokeService> jokeService = new();
        JokeOutput jokeOutput = null!;

        var exception = Record.Exception(() => new Jester(jokeService.Object, jokeOutput));

        var argNullException = Assert.IsType<ArgumentNullException>(exception);
        Assert.Equal(nameof(jokeOutput), argNullException.ParamName);
    }

    [Fact]
    public void OutputJokeWritesJokeToConsole()
    {
        TextWriter original = Console.Out;
        using StringWriter writer = new();
        Console.SetOut(writer);
        try
        {
            new JokeOutput().OutputJoke("This is a joke");
        }
        finally
        {
            Console.SetOut(original);
        }

        Assert.Equal("This is a joke" + Environment.NewLine, writer.ToString());
    }
}
