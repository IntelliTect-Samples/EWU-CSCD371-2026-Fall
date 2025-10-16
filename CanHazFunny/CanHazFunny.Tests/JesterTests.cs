using System;
using Xunit;
using static CanHazFunny.Tests.ChuckNorrisMockJokeService;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void Jester_CanBeCreatedWithValidDependencies()
    {
        // Arrange
        var output = new MockOutput();
        var jokeService = new MockJokeService();
        // Act
        var jester = new Jester(output, jokeService);
        // Assert
        Assert.IsType<Jester>(jester);
    }

    [Fact]
    public void Jester_ThrowsArgumentNullException_WhenOutputIsNull()
    {
        // Arrange
        IOutput? output = null;
        var jokeService = new MockJokeService();
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(output, jokeService));
    }

    [Fact]
    public void Jester_ThrowsArgumentNullException_WhenJokeServiceIsNull()
    {
        // Arrange
        var output = new MockOutput();
        IJokeService? jokeService = null;
        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(output, jokeService));
    }

    [Fact]
    public void Jester_TellJoke_CallsOutputWriteLine()
    {
        // Arrange
        var output = new MockOutput();
        var jokeService = new MockJokeService();
        var jester = new Jester(output, jokeService);
        // Act
        jester.TellJoke();
        // Assert
        Assert.True(output.WriteLineCalled);
        Assert.Equal("This is a mock joke.", output.LastWrittenLine);
    }

    [Fact]
    public void Jester_TellJoke_DoesNotContainChuckNorris()
    {
        // Arrange
        var output = new MockOutput();
        var jokeService = new ChuckNorrisMockJokeService();
        var jester = new Jester(output, jokeService);
        // Act
        jester.TellJoke();
        // Assert
        Assert.True(output.WriteLineCalled);
        Assert.DoesNotContain("Chuck Norris", output.LastWrittenLine ?? string.Empty);
        Assert.Equal("This is a regular joke.", output.LastWrittenLine);
    }

    [Fact]
    public void Jester_TellJoke_TimesOutAfter5Seconds()
    {
        // Arrange
        var output = new MockOutput();
        var jokeService = new OnlyChuckNorrisJokesMockJokeService();
        var jester = new Jester(output, jokeService);
        // Act
        jester.TellJoke();
        // Assert
        Assert.False(output.WriteLineCalled, "Output should not be called when a valid joke is not found before timeout.");
        Assert.Null(output.LastWrittenLine);
    }

    [Fact]
    public void Jester_TellJoke_JokeServiceThrowsError_NoOutput()
    {
        //Arrange
        var output = new MockOutput();
        var jokeService = new ErrorThrowingMockJokeService();
        var jester = new Jester(output, jokeService);
        // Act
        jester.TellJoke();
        // Assert
        Assert.False(output.WriteLineCalled, "Output should not be called when JokeService throws an error.");
        Assert.Null(output.LastWrittenLine);
    }
}

public class MockOutput : IOutput
{
    public bool WriteLineCalled { get; private set; }
    public string? LastWrittenLine { get; private set; }

    public void Write(string message)
    {
        WriteLineCalled = true;
        LastWrittenLine = message;
    }
}

public class MockJokeService : IJokeService
{
    public string GetJoke()
    {
        return "This is a mock joke.";
    }
}

public class ChuckNorrisMockJokeService : IJokeService
{
    private int callCount;

    public string GetJoke()
    {
        if (callCount == 0)
        {
            callCount++;
            return "This is a Chuck Norris joke.";
        }
        else
        {
            return "This is a regular joke.";
        }
    }

    public class OnlyChuckNorrisJokesMockJokeService : IJokeService
    {
        public string GetJoke()
        {
            System.Threading.Thread.Sleep(10);
            return "The joke train is full of Chuck Norris jokes.";
        }
    }

    public class ErrorThrowingMockJokeService : IJokeService
    {
        private int callCount;
        public string GetJoke()
        {
            if (callCount == 0)
            {
                callCount++;
                throw new InvalidOperationException("Simulated API failure.");
            }

            return string.Empty;
        }
    }
}