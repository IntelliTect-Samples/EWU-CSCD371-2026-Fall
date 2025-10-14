using System;
using System.Text.Json;
using Xunit;

namespace CanHazFunny.Tests;

public class JokeServiceTests
{
    [Fact]
    public void GetJoke_ReturnsNonEmptyString()
    {
        // Arrange
        var jokeService = new JokeService();

        // Act
        string joke = jokeService.GetJoke();

        // Assert
        Assert.False(string.IsNullOrWhiteSpace(joke), "The joke should not be null or empty.");
    }

    [Fact]
    public void Deserialize_ValidJson_ReturnsJokeResponse()
    {
        // Arrange
        string json = "{\"joke\":\"Test joke.\"}";

        // Act
        JokeService.JokeResponse? result = JsonSerializer.Deserialize<JokeService.JokeResponse>(json);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Test joke.", result!.Joke);
    }

}
