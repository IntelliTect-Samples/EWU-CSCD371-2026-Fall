using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Tests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void Constructor_CustomDelegates_ReadsAndWritesThroughProperties()
    {
        List<string?> lines = [];
        Program program = new()
        {
            ReadLine = () => "Input",
            WriteLine = lines.Add
        };

        string? input = program.ReadLine();
        program.WriteLine(input);

        Assert.AreEqual<string?>("Input", input);
        Assert.AreEqual<string?>("Input", Assert.ContainsSingle(lines));
    }

    [TestMethod]
    [ResourceLock(WellKnownResources.Console)]
    public void Main_DefaultDelegates_ReadsFromConsoleAndWritesResult()
    {
        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;
        using StringReader input = new("3 + 4");
        using StringWriter output = new();

        try
        {
            Console.SetIn(input);
            Console.SetOut(output);

            Program.Main();
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }

        string[] lines = output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);

        Assert.HasCount(2, lines);
        Assert.AreEqual<string>("7", lines[1]);
    }

    [TestMethod]
    public void Run_MultipleCalculations_WritesEachResult()
    {
        string[] lines = RunWithInput("3 + 4\n42 - 2\n-3 * 4\n12 / -4");

        CollectionAssert.AreEqual(new[]
        {
            "Enter a calculation (for example, 3 + 4), or press Enter to exit.",
            "7", "40", "-12", "-3"
        }, lines);
    }

    [TestMethod]
    public void Run_UnevenDivision_WritesFractionalResult()
    {
        string[] lines = RunWithInput("7 / 2");

        Assert.HasCount(2, lines);
        Assert.AreEqual<decimal>(3.5M, decimal.Parse(lines[1], CultureInfo.CurrentCulture));
    }

    [TestMethod]
    [DataRow("bad input")]
    [DataRow("3 / 0")]
    [DataRow("   ")]
    public void Run_InvalidCalculation_ReportsErrorAndContinues(string calculation)
    {
        string[] lines = RunWithInput($"{calculation}\n3 + 4");

        Assert.HasCount(3, lines);
        Assert.AreEqual<string>(
            "Invalid calculation. Use two integers and +, -, *, or /; division by zero is not allowed.", lines[1]);
        Assert.AreEqual<string>("7", lines[2]);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("\n3 + 4")]
    public void Run_EndOfInputOrEmptyLine_ExitsWithoutCalculation(string input)
    {
        string[] lines = RunWithInput(input);

        Assert.AreEqual<string>(
            "Enter a calculation (for example, 3 + 4), or press Enter to exit.", Assert.ContainsSingle(lines));
    }

    private static string[] RunWithInput(string text)
    {
        using StringReader input = new(text);
        using StringWriter output = new();
        Program program = new()
        {
            ReadLine = input.ReadLine,
            WriteLine = output.WriteLine
        };

        program.Run();

        return output.ToString().Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
    }
}
