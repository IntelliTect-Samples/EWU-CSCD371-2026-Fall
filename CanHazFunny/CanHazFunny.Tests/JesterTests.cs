using Moq;
using System;
using Xunit;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void JesterConstructor_ValidParameters_CreatesJesterInstance()
    {
        // Arrange
        Mock<IJokeService> jokeServiceMock = new();
        Mock<IOutputService> outputServiceMock = new();

        // Act
        Jester jester = new(outputServiceMock.Object, jokeServiceMock.Object);

        // Assert
        Assert.NotNull(jester);
    }

    [Fact]
    public void JesterConstructor_OutputServiceIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Mock<IJokeService> jokeServiceMock = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(null!, jokeServiceMock.Object));
    }

    [Fact]
    public void JesterConstructor_JokeServiceIsNull_ThrowsArgumentNullException()
    {
        // Arrange
        Mock<IOutputService> outputServiceMock = new();

        // Act & Assert
        Assert.Throws<ArgumentNullException>(() => new Jester(outputServiceMock.Object, null!));
    }
}
