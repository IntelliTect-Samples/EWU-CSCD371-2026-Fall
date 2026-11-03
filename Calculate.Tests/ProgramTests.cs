
using Calculate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Tests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void Program_Delegates_CanBeSetAtConstructionAndInvoked()
    {
        string? writtenLine = null;

        var program = new Program
        {
            WriteLine = message => writtenLine = message,
            ReadLine = () => "input text"
        };

        program.WriteLine("output text");
        string? input = program.ReadLine();

        Assert.AreEqual<string?>("output text", writtenLine);
        Assert.AreEqual<string?>("input text", input);
    }

    [TestMethod]
    public void Program_Run_ValidExpression_WritesResult()
    {
        var output = new List<string>();

        var program = new Program
        {
            WriteLine = output.Add,
            ReadLine = () => "3 + 4"
        };

        program.Run();

        #pragma warning disable MSTEST0037
        Assert.AreEqual<int>(2, output.Count);
        #pragma warning restore MSTEST0037

        Assert.AreEqual<string>(
            "Result: 7",
            output[1]);
    }

    [TestMethod]
    public void Program_Run_InvalidExpression_WritesError()
    {
        var output = new List<string>();

        var program = new Program
        {
            WriteLine = output.Add,
            ReadLine = () => "3 + four"
        };

        program.Run();

        #pragma warning disable MSTEST0037
        Assert.AreEqual<int>(2, output.Count);
        #pragma warning restore MSTEST0037

        Assert.AreEqual<string>(
            "Invalid expression.",
            output[1]);
    }

    [TestMethod]
    [DoNotParallelize]
    public void Program_Run_UsesDefaultConsoleDelegates()
    {
        TextReader originalInput = Console.In;
        TextWriter originalOutput = Console.Out;

        using var input = new StringReader("3 + 4");
        using var output = new StringWriter();

        try
        {
            Console.SetIn(input);
            Console.SetOut(output);

            var program = new Program();
            program.Run();
        }
        finally
        {
            Console.SetIn(originalInput);
            Console.SetOut(originalOutput);
        }

        string[] lines = output.ToString().Split(
            Environment.NewLine,
            StringSplitOptions.RemoveEmptyEntries);

        #pragma warning disable MSTEST0037
        Assert.AreEqual<int>(2, lines.Length);
        #pragma warning restore MSTEST0037

        Assert.AreEqual<string>(
            "Result: 7",
            lines[1]);
    }
}
