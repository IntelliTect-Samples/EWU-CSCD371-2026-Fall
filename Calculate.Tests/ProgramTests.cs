using Calculate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;

namespace Calculate.Tests;

[TestClass]
public class ProgramTests
{
    private static readonly object ConsoleLock = new();
    /// <summary>
    /// Helper method to run Program.Main with specified input and capture output.
    /// </summary>
    private static string RunProgramWithInput(string input)
    {
        lock (ConsoleLock)
        {
            StringReader reader = new StringReader(input);
            StringWriter writer = new StringWriter();
            TextReader originalIn = Console.In;
            TextWriter originalOut = Console.Out;

            try
            {
                Console.SetIn(reader);
                Console.SetOut(writer);

                Program.Main();

                string output = writer.ToString();
                return output;
            }
            finally
            {
                Console.SetIn(originalIn);
                Console.SetOut(originalOut);
            }
        }
    }

    [TestMethod]
    public void Main_ValidIntegerCalculation_PrintsResult()
    {
        string input = "3 + 3\n";
        string output = RunProgramWithInput(input);
        StringAssert.Contains(output, "The result is: 6");
    }

    [TestMethod]
    public void Main_ValidDoubleCalculation_PrintsResult()
    {
        string input = "7.5 + 2.5";
        string output = RunProgramWithInput(input);
        StringAssert.Contains(output, "The result is: 10");
    }

    [TestMethod]
    public void Main_InvalidCalculation_OutputsErrorMessage()
    {
        string input = "a + 3";
        string output = RunProgramWithInput(input);
        StringAssert.Contains(output, "Something went wrong with the calculation");
    }

    [TestMethod]
    public void Main_NoInput_OutputsNoInputMessage()
    {
        string input = string.Empty;
        string output = RunProgramWithInput(input);
        StringAssert.Contains(output, "There wasn't any input");
    }

    [TestMethod]
    public void WriteLine_OutputsExpectedText_WhenDelegateIsSet()
    {
        string output = string.Empty;
        var program = new Program
        {
            WriteLine = s => output = s
        };
        string input = "Hello World!";
        
        program.WriteLine(input);
  
        Assert.AreEqual<string>(input, output);
    }

    [TestMethod]
    public void ReadLine_ReturnsExpectedInput_WhenDelegateIsSet()
    {
        string input = "Hello";
        var program = new Program
        {
            ReadLine = () => input
        };

        string? receivedInput = program.ReadLine();

        Assert.AreEqual<string>(input, receivedInput);
    }

    




}
