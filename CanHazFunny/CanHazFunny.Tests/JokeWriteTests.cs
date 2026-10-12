using System;
using System.IO;
using Xunit;

namespace CanHazFunny.Tests;

[CollectionDefinition("Console tests", DisableParallelization = true)]
public class ConsoleTestCollection
{
}

[Collection("Console tests")]
public class JokeWriteTests
{
    [Fact]
    public void WriteJoke_WhenCalled_WritesJokeFollowedByNewline()
    {
        TextWriter originalOutput = Console.Out;
        using var capturedOutput = new StringWriter();

        try
        {
            Console.SetOut(capturedOutput);

            var writer = new JokeWrite();
            writer.WriteJoke("Acceptable");

            Assert.Equal("Acceptable" + Environment.NewLine, capturedOutput.ToString());
        }
        finally
        {
            Console.SetOut(originalOutput);
        }
    }
}
