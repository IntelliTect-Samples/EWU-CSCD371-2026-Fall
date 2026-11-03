using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Tests;

[TestClass]
public sealed class ProgramTests
{
    [TestMethod]
    public void Program_CustomDelegates_WorkCorrectly()
    {
        // Arrange
        List<string> output = [];

        Program program = new()
        {
            WriteLine = message => output.Add(message),
            ReadLine = () => "Hello"
        };

        // Act
        program.WriteLine("Test message");
        string? input = program.ReadLine();

        // Assert
        Assert.AreEqual<string>("Test message", output[0]);
        Assert.AreEqual<string?>("Hello", input);
    }

    [TestMethod]
    public void Program_DefaultConstructor_UsesConsoleDelegates()
    {
        // Arrange
        Program program = new();

        Action<string> expectedWrite = Console.WriteLine;
        Func<string?> expectedRead = Console.ReadLine;

        // Assert
        Assert.AreEqual<Action<string>>(
            expectedWrite, program.WriteLine);

        Assert.AreEqual<Func<string?>>(
            expectedRead, program.ReadLine);
    }

    [TestMethod]
    public void Run_ValidInput_PrintsCorrectResult()
    {
        // Arrange
        List<string> output = [];

        Program program = new()
        {
            ReadLine = () => "3 + 4",
            WriteLine = message => output.Add(message)
        };

        // Act
        program.Run();

        // Assert
        Assert.AreEqual<string>("Result: 7", output[^1]);
    }

    [TestMethod]
    public void Run_InvalidInput_PrintsError()
    {
        // Arrange
        List<string> output = [];

        Program program = new()
        {
            ReadLine = () => "3+4",
            WriteLine = message => output.Add(message)
        };

        // Act
        program.Run();

        // Assert
        Assert.AreEqual<string>(
            "Invalid calculation.", output[^1]);
    }
}
