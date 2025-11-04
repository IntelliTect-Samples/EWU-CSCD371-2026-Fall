using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculate;
using System.Runtime.InteropServices;


namespace Calculate.Tests;

[TestClass]
public sealed class CalculatorTests
{
    [TestMethod]
    public void MathematicalOperations_EvaluatingValidInts_ReturnsCorrectValues()
    {
        var calc = new Calculator<int>();

        int sum = calc.MathematicalOperations['+'](6, 7);
        int difference = calc.MathematicalOperations['-'](6, 7);
        int product = calc.MathematicalOperations['*'](6, 7);
        int quotient = calc.MathematicalOperations['/'](6, 7);

        Assert.AreEqual<int>(13, sum);
        Assert.AreEqual<int>(-1, difference);
        Assert.AreEqual<int>(42, product);
        Assert.AreEqual<int>(0, quotient);
    }

    [TestMethod]
    public void MathematicalOperations_ValidDecimalMath_ReturnsCorrectValues()
    {
        var calc = new Calculator<double>();

        double sum = calc.MathematicalOperations['+'](6.7, 6.7);
        double difference = calc.MathematicalOperations['-'](6.7, 6.7);
        double product = calc.MathematicalOperations['*'](6.7, 6.7);
        double quotient = calc.MathematicalOperations['/'](6.7, 6.7);

        Assert.AreEqual<double>(13.4, sum);
        Assert.AreEqual<double>(0, difference);
        Assert.AreEqual<double>(44.89, product);
        Assert.AreEqual<double>(1, quotient);
    }

    [TestMethod]
    public void TryCalculate_EvaluatingInvalidExpressions_ReturnsFalse()
    {
        var calc = new Calculator<int>();

        bool sum = calc.TryCalculate("6+ 7", out _);
        bool difference = calc.TryCalculate("6-7", out _);
        bool product = calc.TryCalculate("6 *7", out _);
        bool quotient = calc.TryCalculate("six / 7", out _);

        // Using 'IsFalse' here to avoid compiler warnings
        Assert.IsFalse(sum);
        Assert.IsFalse(difference);
        Assert.IsFalse(product);
        Assert.IsFalse(quotient);
    }

    [TestMethod]
    public void TryCalculate_DivideByZero_ReturnsFalse()
    {
        var calc = new Calculator<int>();

        bool divZero = calc.TryCalculate("6 / 0", out _);

        // Using 'IsFalse' here to avoid compiler warnings
        Assert.IsFalse(divZero);
    }

}
