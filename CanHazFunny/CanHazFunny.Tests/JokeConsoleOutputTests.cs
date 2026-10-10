
using System;
using System.IO;
using CanHazFunny;
using Xunit;

namespace CanHazFunny.Tests;

public class JokeConsoleOutputTests
{
    [Fact]
    public void WriteLine_WritesJokeWithSurroundingBlankLines()
    {
        const string expectedJoke = "This is a test joke.";
        TextWriter originalOutput = Console.Out;

        using StringWriter capturedOutput = new();

        try
        {
            Console.SetOut(capturedOutput);

            JokeOutput output = new();
            output.WriteLine(expectedJoke);
        }
        finally
        {
            Console.SetOut(originalOutput);
        }

        Assert.Equal(
            $"\n{expectedJoke}\n{Environment.NewLine}",
            capturedOutput.ToString());
    }

    [Fact]
    public void WriteLine_ThrowsWhenJokeIsNull()
    {
        JokeOutput output = new();

        ArgumentNullException exception =
            Assert.Throws<ArgumentNullException>(
                () => output.WriteLine(null!));

        Assert.Equal("joke", exception.ParamName);
    }
}
