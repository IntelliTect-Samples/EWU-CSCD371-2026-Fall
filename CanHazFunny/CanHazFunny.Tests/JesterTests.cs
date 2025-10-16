using Xunit;
using System;
using System.IO;
using CanHazFunny;
using System.Collections.Generic;

namespace CanHazFunny.Tests;

public class JesterTests
{
    [Fact]
    public void Constructor_ValidParameters_AllocatesNewInstance()
    {
        Jester jester = new Jester(new GoodJokeTestClass(), new ConsoleOutput());
        Assert.NotNull(jester);
    }

    [Fact]
    public void Constructor_NullJokeService_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Jester(null, new ConsoleOutput()));
    }

    [Fact]
    public void Constructor_NullJokeOutput_ThrowsArgumentNullException()
    {
        Assert.Throws<ArgumentNullException>(() => new Jester(new GoodJokeTestClass(), null));
    }

    [Fact]
    public void TellJoke_JokeOutput_JokeOutputToStandardOut()
    {
        TextWriter originalOut = Console.Out;
        using StringWriter sw = new StringWriter();
        Console.SetOut(sw);
        Jester jester = new Jester(new GoodJokeTestClass(), new ConsoleOutput());
        jester.TellJoke();
        Console.SetOut(originalOut);
        Assert.Equal("Funny joke that does not contain the illegal term.", sw.ToString().Trim());
    }

    [Fact]
    public void TellJoke_GoodJokeGiven_GoodJokeOutputToConsole()
    {
        TestOutput tester = new TestOutput();
        Jester jester = new Jester(new GoodJokeTestClass(), tester);
        jester.TellJoke();
        Assert.Equal("Funny joke that does not contain the illegal term.", tester.Output[0]);
    }

    [Fact]
    public void TellJoke_BadJokeGiven_GoodJokeOutputToConsole()
    {
        TestOutput tester = new TestOutput();
        Jester jester = new Jester(new BadJokeTestClass(), tester);
        jester.TellJoke();
        Assert.Equal("A good joke.", tester.Output[0]);
    }
}

public class GoodJokeTestClass : IJokeService
{
    public string GetJoke()
    {
        return "Funny joke that does not contain the illegal term.";
    }
}

public class BadJokeTestClass : IJokeService
{
    readonly Queue<string> jokes;
    public BadJokeTestClass()
    {
        jokes = new Queue<string>();
        jokes.Enqueue("Chuck Norris can divide by zero.");
        jokes.Enqueue("Chuck Norris counted to infinity. Twice.");
        jokes.Enqueue("Chuck Norris can slam a revolving door.");
        jokes.Enqueue("A good joke.");
    }
    public string GetJoke()
    {
        return jokes.Dequeue();
    }
}

public class TestOutput : IOutput
{
    public List<string> Output { get; } = new List<string>();
    public void WriteLine(string message)
    {
        Output.Add(message);
    }
}