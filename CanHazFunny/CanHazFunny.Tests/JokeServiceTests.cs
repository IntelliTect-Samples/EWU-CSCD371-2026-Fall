using Xunit;

namespace CanHazFunny.Tests;

public class JokeServiceTests
{
    [Fact]
    public void GetJoke_ReturnsNonEmptyString()
    {
        // Arrange
        JokeService jokeService = new();

        // Act
        string joke = jokeService.GetJoke();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(joke));
    }
}