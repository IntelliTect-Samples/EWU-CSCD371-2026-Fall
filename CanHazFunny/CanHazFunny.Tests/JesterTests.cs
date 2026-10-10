
using System;
using CanHazFunny;
using Moq;
using Xunit;

namespace CanHazFunny.Tests;

public class JesterTests
{
    /// <summary>
    /// Tests that the constructor of the <see cref="Jester"/> class throws an <see cref="ArgumentNullException"/> when the <paramref name="jokeOutput"/> parameter is null.
    /// </summary>
    /// <remarks>
    /// This test verifies that the <see cref="Jester"/> constructor correctly handles a null <paramref name="jokeOutput"/> parameter by throwing an <see cref="ArgumentNullException"/>. It uses the Moq library to create a mock implementation of the <see cref="IJokeService"/> interface, which is passed to the constructor along with a null value for the <paramref name="jokeOutput"/> parameter. The test asserts that the exception thrown has the expected parameter name.
    /// </remarks>
    [Fact]
    public void ConstructorThrowsWhenOutputIsNull()
    {
        Mock<IJokeService> jokeService = new();

        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(
                () => new Jester(jokeService.Object, null!));

        Assert.Equal("jokeOutput", exception.ParamName);
    }

    /// <summary>
    /// Tests that the constructor of the <see cref="Jester"/> class throws an <see cref="ArgumentNullException"/> when the <paramref name="jokeService"/> parameter is null.
    /// </summary>
    /// <remarks>
    /// This test verifies that the <see cref="Jester"/> constructor correctly handles a null <paramref name="jokeService"/> parameter by throwing an <see cref="ArgumentNullException"/>. It uses the Moq library to create a mock implementation of the <see cref="IJokeConsoleOutput"/> interface, which is passed to the constructor along with a null value for the <paramref name="jokeService"/> parameter. The test asserts that the exception thrown has the expected parameter name.
    /// </remarks>
    [Fact]
    public void ConstructorThrowsWhenJokeServiceIsNull()
    {
        Mock<IJokeConsoleOutput> output = new();

        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(
                () => new Jester(null!, output.Object));

        Assert.Equal("jokeService", exception.ParamName);
    }

    /// <summary>
    /// Tests that the <see cref="Jester.TellJoke"/> method outputs an accepted joke when the joke service returns a valid joke.
    /// </summary>
    /// <remarks>
    /// This test verifies that the <see cref="Jester.TellJoke"/> method correctly retrieves a joke from the joke service and outputs it to the console when the joke service returns a valid joke. It uses the Moq library to create mock implementations of the <see cref="IJokeService"/> and <see cref="IJokeConsoleOutput"/> interfaces. The test sets up the joke service to return a specific joke and asserts that the output method is called with the expected joke.
    /// </remarks>
    [Fact]
    public void TellJokeOutputsAcceptedJoke()
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

    /// <summary>
    /// Tests that the <see cref="Jester.TellJoke"/> method skips forbidden jokes until an acceptable joke is found.
    /// </summary>
    /// <remarks>
    /// This test verifies that the <see cref="Jester.TellJoke"/> method correctly skips jokes containing forbidden words ("Chuck", "Norris", or "Texas Ranger") and continues to retrieve jokes until an acceptable one is found. It uses the Moq library to create mock implementations of the <see cref="IJokeService"/> and <see cref="IJokeConsoleOutput"/> interfaces. The test sets up the joke service to return a sequence of jokes, some of which contain forbidden words, and asserts that the output method is called with the expected accepted joke after skipping the forbidden ones.
    /// </remarks>
    [Fact]
    public void TellJokeSkipsForbiddenJokesUntilAcceptableJokeIsFound()
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

    /// <summary>
    /// Tests that the <see cref="Jester.TellJoke"/> method outputs an error message and stops when the joke service throws an exception.
    /// </summary>
    /// <remarks>
    /// This test verifies that the <see cref="Jester.TellJoke"/> method correctly handles exceptions thrown by the joke service. It uses the Moq library to create mock implementations of the <see cref="IJokeService"/> and <see cref="IJokeConsoleOutput"/> interfaces. The test sets up the joke service to throw an <see cref="InvalidOperationException"/> with a specific error message and asserts that the output method is called with the expected error message and that no further jokes are retrieved after the exception is thrown.
    /// </remarks>
    [Fact]
    public void TellJokeWhenJokeServiceThrowsOutputsErrorAndStops()
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
