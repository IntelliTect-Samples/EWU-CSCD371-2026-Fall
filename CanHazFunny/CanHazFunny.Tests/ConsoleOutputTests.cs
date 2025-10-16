using System;
using System.IO;
using Xunit;


namespace CanHazFunny.Tests;

public class ConsoleOutputTests
{

    [Fact]
    public void WriteLine_WithMessage_WritesMessageAndNewLine()
    {
        ConsoleOutput consoleOutput = new ConsoleOutput();
        TextWriter original = Console.Out;
        using StringWriter sw = new StringWriter();
        Console.SetOut(sw);
        consoleOutput.WriteLine("hello");
        Console.SetOut(original);
        Assert.Equal("hello" + Environment.NewLine, sw.ToString());
    }

    [Fact]
    public void WriteLine_WithEmptyString_WritesNewLineOnly()
    {
        ConsoleOutput consoleOutput = new ConsoleOutput();
        TextWriter original = Console.Out;
        using StringWriter sw = new StringWriter();
        Console.SetOut(sw);
        consoleOutput.WriteLine(string.Empty);
        Console.SetOut(original);
        Assert.Equal(Environment.NewLine, sw.ToString());
    }

    [Fact]
    public void WriteLine_OnMultipleCalls_AppendsEachOnNewLine()
    {
        ConsoleOutput consoleOutput = new ConsoleOutput();
        TextWriter original = Console.Out;
        using StringWriter sw = new StringWriter();
        Console.SetOut(sw);
        consoleOutput.WriteLine("first");
        consoleOutput.WriteLine("second");
        Console.SetOut(original);
        Assert.Equal($"first{Environment.NewLine}second{Environment.NewLine}", sw.ToString());
    }
}
