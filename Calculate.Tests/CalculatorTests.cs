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
}