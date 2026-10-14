using System;
using System.IO;
using Xunit;

namespace CanHazFunny.Tests;

public class ConsoleJokeOutputTests
{
    private const string Joke = "Why do programmers prefer dark mode? Because light attracts bugs.";
    [Fact]
    public void WriteJoke_WritesJokeToConsole()
    {
        TextWriter originalOut = Console.Out;
        using StringWriter writer = new();
        Console.SetOut(writer);

        try
        {
            new ConsoleJokeOutput().WriteJoke(Joke);
        }
        finally
        {
            Console.SetOut(originalOut);
        }

        Assert.Equal(Joke + Environment.NewLine, writer.ToString());
    }
}
