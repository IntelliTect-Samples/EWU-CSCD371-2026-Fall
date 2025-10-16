using Xunit;
using System;
using System.IO;
using CanHazFunny;
using System.Reflection;

namespace CanHazFunny.Tests;

public class JokeServiceTests
{
    [Fact]
    public void GetJoke_ShouldReturnNonEmptyString()
    {
        JokeService jokeService = new JokeService();
        string joke = jokeService.GetJoke();
        Assert.False(string.IsNullOrEmpty(joke), "JokeService returned an empty or whitespace string.");
        Assert.IsType<string>(joke);
    }
}