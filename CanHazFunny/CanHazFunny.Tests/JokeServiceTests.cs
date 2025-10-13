using System;
using Xunit;

namespace CanHazFunny.Tests
{
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
    }
}
