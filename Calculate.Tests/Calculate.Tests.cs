using Calculate;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    [DataRow("3 + 4", 7f)]
    [DataRow("42 - 2", 40f)]
    [DataRow("6 * 7", 42f)]
    [DataRow("7 / 2", 3.5f)]
    [DataRow("3    +    4", 7f)]
    [DataRow("-3 + 4", 1f)]
    [DataRow("3 + -4", -1f)]
    public void TryCalculate_ValidExpressions_ReturnsResult(
        string expression, float expected)
    {
        var calculator = new Calculator();

        Assert.IsTrue(calculator.TryCalculate(expression, out float result));
        Assert.AreEqual<float>(expected, result);
    }

    [TestMethod]
    [DataRow("3+4")]
    [DataRow("3 +4")]
    [DataRow("3+ 4")]
    [DataRow("3 ++ 4")]
    [DataRow("3 + 4 extra")]
    [DataRow("3.5 + 4")]
    [DataRow("3 + four")]
    [DataRow("")]
    [DataRow("   ")]
    public void TryCalculate_InvalidExpressions_ReturnsFalse(string expression)
    {
        var calculator = new Calculator();

        Assert.IsFalse(calculator.TryCalculate(expression, out float result));
        Assert.AreEqual<float>(0f, result);
    }

    [TestMethod]
    public void TryCalculate_NullExpression_ReturnsFalse()
    {
        var calculator = new Calculator();

        Assert.IsFalse(calculator.TryCalculate(null, out float result));
        Assert.AreEqual<float>(0f, result);
    }

    [TestMethod]
    public void TryCalculate_DivisionByZero_ThrowsException()
    {
        var calculator = new Calculator();

        Assert.ThrowsExactly<DivideByZeroException>(
            () => calculator.TryCalculate("3 / 0", out _));
    }

    [TestMethod]
    public void Program_Run_UsesInjectedDelegates()
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
            "Enter a mathematical expression (e.g., 3 + 4):",
            output[0]);
        Assert.AreEqual<string>("Result: 7", output[1]);
    }

    [TestMethod]
    [DoNotParallelize]
    public void Program_Run_UsesConsoleByDefault()
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

        Assert.AreEqual<string>(
            "Enter a mathematical expression (e.g., 3 + 4):",
            lines[0]);

        Assert.AreEqual<string>("Result: 7", lines[1]);
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

        Assert.AreEqual<string>("Invalid expression.", output[1]);
    }
}