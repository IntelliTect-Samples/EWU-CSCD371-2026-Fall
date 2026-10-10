
using System;
using System.IO;
using CanHazFunny;
using Xunit;

namespace CanHazFunny.Tests;

public class JokeConsoleOutputTests
{
    /// <summary>
    /// Tests that the <see cref="JokeOutput.WriteLine"/> method writes a joke to the console with surrounding blank lines.
    /// </summary>
    /// <remarks>
    /// This test verifies that the <see cref="JokeOutput.WriteLine"/> method correctly formats the output by adding a blank line before and after the joke. It captures the console output using a <see cref="StringWriter"/> and asserts that the output matches the expected format.
    /// </remarks>
    /// <seealso cref="JokeOutput.WriteLine(string)"/>
    /// <seealso cref="JokeOutput"/>
    /// <seealso cref="IJokeConsoleOutput"/>
    /// <seealso cref="Console"/>
    /// <seealso cref="StringWriter"/>
    /// <seealso cref="TextWriter"/>
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

    /// <summary>
    /// Tests that the <see cref="JokeOutput.WriteLine"/> method throws an <see cref="ArgumentNullException"/> when the <paramref name="joke"/> parameter is null.
    /// </summary>
    /// <remarks>
    /// This test verifies that the <see cref="JokeOutput.WriteLine"/> method correctly handles a null <paramref name="joke"/> parameter by throwing an <see cref="ArgumentNullException"/>. It asserts that the exception thrown has the expected parameter name.
    /// </remarks>
    /// <seealso cref="JokeOutput.WriteLine(string)"/>
    /// <seealso cref="JokeOutput"/>
    /// <seealso cref="IJokeConsoleOutput"/>
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
