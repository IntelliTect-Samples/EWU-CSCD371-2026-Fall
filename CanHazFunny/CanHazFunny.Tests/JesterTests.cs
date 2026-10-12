using Moq;
using System;
using Xunit;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact] // marks this method as an xUnit test.
    public void Constructor_NullOutput_ThrowsException()
    {
        // Arrange
        var jokeService = new Mock<IJokeService>(); // Mock<IJokeService>() creates a fake joke service, so we don't need to call the actual API.

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() => // verifies that your constructor rejects the null dependency.
            new Jester(null!, jokeService.Object)); // null! deliberately passes null while suppressing a compiler warning
    }

    [Fact]
    public void Constructor_NullJokeService_ThrowsException()
    {
        // Arrange
        var output = new Mock<IJokeOutput>();

        // Act and Assert
        Assert.Throws<ArgumentNullException>(() =>
            new Jester(output.Object, null!));
    }

    [Fact]
    public void TellJoke_ValidJoke_DisplaysJoke()
    {
        // Arrange
        var jokeService = new Mock<IJokeService>();
        var output = new Mock<IJokeOutput>();

        jokeService.Setup(service => service.GetJoke())
            .Returns("Why did the chicken cross the road?");

        var jester = new Jester(output.Object, jokeService.Object);

        // Act
        jester.TellJoke();

        // Assert
        output.Verify(
            o => o.WriteJoke("Why did the chicken cross the road?"),
            Times.Once);
    }

    [Fact]
    public void TellJoke_ChuckNorrisJoke_SkipsAndGetsAnother()
    {
        // Arrange
        var jokeService = new Mock<IJokeService>();
        var output = new Mock<IJokeOutput>();

        jokeService.SetupSequence(service => service.GetJoke())
            .Returns("Chuck Norris can divide by zero.")
            .Returns("Why did the chicken cross the road?");

        var jester = new Jester(output.Object, jokeService.Object);

        // Act
        jester.TellJoke();

        // Assert
        output.Verify(
            o => o.WriteJoke("Why did the chicken cross the road?"),
            Times.Once);

        jokeService.Verify(
            service => service.GetJoke(),
            Times.Exactly(2));
    }
}