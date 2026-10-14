using System;
using System.IO;
using Xunit;

namespace CanHazFunny.Tests;

public class ConsoleJokeOutputTests
{
    [Fact]
    public void OutputJoke_WritesJokeToConsole()
    {
        using var writer = new StringWriter();
        TextWriter originalOutput = Console.Out;

        try
        {
            Console.SetOut(writer);

            var output = new ConsoleJokeOutput();
            output.OutputJoke("Test joke");

            Assert.Equal(
                "Test joke" + Environment.NewLine,
                writer.ToString());
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }
}
