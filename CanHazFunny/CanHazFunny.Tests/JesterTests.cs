using System;
using Moq;
using Xunit;

namespace CanHazFunny.Tests;

/// <summary>
/// Unit tests for the Jester class. These tests verify that the Jester correctly retrieves jokes from the IJokeService and passes them to the IOutputService, while also ensuring that jokes mentioning "Chuck Norris" are skipped. The tests use mock implementations of IJokeService and IOutputService to ensure that the Jester behaves as expected without relying on external services or console output.
/// </summary>
public class JesterTests
{
    /// <summary>
    /// Tests that the TellJoke method retrieves a joke from the IJokeService and passes it to the IOutputService, while skipping jokes that mention "Chuck Norris". The test uses mock implementations of IJokeService and IOutputService to verify that the Jester correctly interacts with these services, ensuring that jokes mentioning "Chuck Norris" are not output.
    /// </summary>
    [Fact]
    public void TellJokeSkipsChuckNorrisJokesOutputsFirstOtherJoke()
    {
        const string joke = "Testing found this joke funny, production never finds it all.";
        Mock<IJokeService> jokeService = new();
        Mock<IOutputService> outputService = new();
        jokeService
            .SetupSequence(service => service.GetJoke())
            .Returns("This joke mentions chuck norris.")
            .Returns(joke);

        Jester jester = new(jokeService.Object, outputService.Object);

        jester.TellJoke();

        jokeService.Verify(service => service.GetJoke(), Times.Exactly(2));
        outputService.Verify(service => service.Output(joke), Times.Once);
        outputService.Verify(
            service => service.Output(It.Is<string>(message =>
                message.Contains("Chuck Norris", StringComparison.OrdinalIgnoreCase))),
            Times.Never);
    }

    /// <summary>
    /// Tests that the Jester constructor throws an ArgumentNullException when either the IJokeService or IOutputService is null. The test uses parameterized inputs to verify that the constructor correctly validates its dependencies and throws the appropriate exception with a message indicating which dependency is null.
    /// </summary>
    /// <param name="useValidJokeService"></param>
    /// <param name="useValidOutputService"></param>
    /// <param name="expected"></param>
    [Theory]
    [InlineData(false, true, "jokeService")]
    [InlineData(true, false, "outputService")]
    public void ConstructorThrows(bool useValidJokeService, bool useValidOutputService, string expected)
    {
        IJokeService? jokeService = useValidJokeService ?
                new Mock<IJokeService>().Object
                : null;

        IOutputService? outputService = useValidOutputService ?
                new Mock<IOutputService>().Object
                : null;

        ArgumentNullException ex = Assert.Throws<ArgumentNullException>(
            () => new Jester(jokeService!, outputService!));

        Assert.Contains(expected, ex.Message);
    }
}
