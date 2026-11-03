using Calculate;

namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void Add_ReturnsSum()
    {
        Assert.AreEqual<int>(7, Calculator.Add(3, 4));
    }

    [TestMethod]
    public void Subtract_ReturnsDifference()
    {
        Assert.AreEqual<int>(2, Calculator.Subtract(6, 4));
    }

    [TestMethod]
    public void Multiply_ReturnsProduct()
    {
        Assert.AreEqual<int>(12, Calculator.Multiply(3, 4));
    }

    [TestMethod]
    public void Divide_ReturnsQuotient()
    {
        Assert.AreEqual<int>(3, Calculator.Divide(9, 3));
    }

    [TestMethod]
    public void MathematicalOperations_InvokesCorrectMethods()
    {
        Calculator calculator = new();

        Assert.AreEqual<int>(7, calculator.MathematicalOperations['+'](3, 4));
        Assert.AreEqual<int>(2, calculator.MathematicalOperations['-'](6, 4));
        Assert.AreEqual<int>(12, calculator.MathematicalOperations['*'](3, 4));
        Assert.AreEqual<int>(3, calculator.MathematicalOperations['/'](9, 3));
    }

    [TestMethod]
    public void TryCalculate_ValidExpression_ReturnsTrue()
    {
        Calculator calculator = new();

        bool success = calculator.TryCalculate("3 + 4", out int result);

        Assert.IsTrue(success);
        Assert.AreEqual<int>(7, result);
    }

    [TestMethod]
    public void TryCalculate_InvalidExpression_ReturnsFalse()
    {
        Calculator calculator = new();

        bool success = calculator.TryCalculate("3+4", out int result);

        Assert.IsFalse(success);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculate_DivisionByZero_ReturnsFalse()
    {
        Calculator calculator = new();

        bool success = calculator.TryCalculate("8 / 0", out int result);

        Assert.IsFalse(success);
        Assert.AreEqual<int>(0, result);
    }


    [TestMethod]
    public void Run_ValidCalculation_WritesResult()
    {
        List<string> output = new();

        Program program = new()
        {
            ReadLine = () => "3 + 4",
            WriteLine = message => output.Add(message)
        };

        program.Run();

        Assert.HasCount(2, output);
        Assert.AreEqual<string>("Result: 7", output[1]);
    }

    [TestMethod]
    public void Run_InvalidCalculation_WritesError()
    {
        List<string> output = new();

        Program program = new()
        {
            ReadLine = () => "3+4",
            WriteLine = message => output.Add(message)
        };

        program.Run();

        Assert.HasCount(2, output);
        Assert.AreEqual<string>("Invalid calculation.", output[1]);
    }

    [TestMethod]
    [DataRow("abc + 4")]
    [DataRow("3 + abc")]
    [DataRow("3 % 4")]
    [DataRow("3 +")]
    [DataRow("")]
    public void TryCalculate_InvalidInputs_ReturnFalse(string input)
    {
        Calculator calculator = new();

        bool success = calculator.TryCalculate(input, out int result);

        Assert.IsFalse(success);
        Assert.AreEqual<int>(0, result);
    }
}