
using System;
using CanHazFunny;
using Moq;
using Xunit;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void Constructor_ThrowsWhenOutputIsNull()
    {
        Mock<IJokeService> jokeService = new();

        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(
                () => new Jester(jokeService.Object, null!));

        Assert.Equal("jokeOutput", exception.ParamName);
    }

    [Fact]
    public void Constructor_ThrowsWhenJokeServiceIsNull()
    {
        Mock<IJokeConsoleOutput> output = new();

        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(
                () => new Jester(null!, output.Object));

        Assert.Equal("jokeService", exception.ParamName);
    }

    [Fact]
    public void TellJoke_OutputsAcceptedJoke()
    {
        const string expectedJoke =
            "Why did the programmer quit? No exceptions.";

        Mock<IJokeConsoleOutput> output = new();
        Mock<IJokeService> jokeService = new();

        jokeService
            .Setup(service => service.GetJoke())
            .Returns(expectedJoke);

        Jester jester = new(jokeService.Object, output.Object);

        jester.TellJoke();

        jokeService.Verify(
            service => service.GetJoke(),
            Times.Once);

        output.Verify(
            destination => destination.WriteLine(expectedJoke),
            Times.Once);
    }

    [Fact]
    public void TellJoke_SkipsForbiddenJokesUntilAcceptableJokeIsFound()
    {
        const string acceptedJoke = "A perfectly ordinary joke.";

        Mock<IJokeConsoleOutput> output = new();
        Mock<IJokeService> jokeService = new();

        jokeService
            .SetupSequence(service => service.GetJoke())
            .Returns("A joke about Chuck.")
            .Returns("A joke about Norris.")
            .Returns("A joke about chuck.")
            .Returns("A joke about NORRIS.")
            .Returns("A joke about a Texas Ranger.")
            .Returns(acceptedJoke);

        Jester jester = new(jokeService.Object, output.Object);

        jester.TellJoke();

        jokeService.Verify(
            service => service.GetJoke(),
            Times.Exactly(6));

        output.Verify(
            destination => destination.WriteLine(It.IsAny<string>()),
            Times.Once);

        output.Verify(
            destination => destination.WriteLine(acceptedJoke),
            Times.Once);
    }

    [Fact]
    public void TellJoke_WhenJokeServiceThrows_OutputsErrorAndStops()
    {
        const string errorMessage = "Service unavailable";

        Mock<IJokeConsoleOutput> output = new();
        Mock<IJokeService> jokeService = new();

        jokeService
            .Setup(service => service.GetJoke())
            .Throws(new InvalidOperationException(errorMessage));

        Jester jester = new(jokeService.Object, output.Object);

        jester.TellJoke();

        jokeService.Verify(
            service => service.GetJoke(),
            Times.Once);

        output.Verify(
            destination => destination.WriteLine(
                $"Error: {errorMessage}"),
            Times.Once);

        output.Verify(
            destination => destination.WriteLine(
                It.IsAny<string>()),
            Times.Once);
    }

}
