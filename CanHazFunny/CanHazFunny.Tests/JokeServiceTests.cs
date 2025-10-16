using Xunit;
namespace CanHazFunny.Tests;

public class JokeServiceTests
{
    [Fact]
    public void GetJoke_JSONResponse_Success()
    {
        // Arrange
        var jokeService = new JokeService();
        // Act
        var joke = jokeService.GetJoke();
        // Assert
        Assert.IsType<string>(joke);
        Assert.DoesNotContain("{", joke);
        Assert.DoesNotContain("}", joke);
    }
}
