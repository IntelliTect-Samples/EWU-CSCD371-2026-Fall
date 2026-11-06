using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void AddReturnsSum() => Assert.AreEqual<int>(7, Calculator.Add(3, 4));

    [TestMethod]
    public void SubtractReturnsDifference() => Assert.AreEqual<int>(40, Calculator.Subtract(42, 2));

    [TestMethod]
    public void MultiplyReturnsProduct() => Assert.AreEqual<int>(12, Calculator.Multiply(3, 4));

    [TestMethod]
    public void DivideReturnsQuotient() => Assert.AreEqual<int>(5, Calculator.Divide(10, 2));

    [TestMethod]
    public void MathematicalOperationsContainsTheFourOperators()
    {
        IReadOnlyDictionary<char, Func<int, int, int>> operations = new Calculator().MathematicalOperations;

        Assert.AreEqual<int>(4, operations.Count);
        foreach (char op in "+-*/")
        {
            Assert.AreEqual<bool>(true, operations.ContainsKey(op));
        }
    }

    [TestMethod]
    public void MathematicalOperationsMapToTheMatchingMethods()
    {
        IReadOnlyDictionary<char, Func<int, int, int>> operations = new Calculator().MathematicalOperations;

        Assert.AreEqual<int>(Calculator.Add(8, 2), operations['+'](8, 2));
        Assert.AreEqual<int>(Calculator.Subtract(8, 2), operations['-'](8, 2));
        Assert.AreEqual<int>(Calculator.Multiply(8, 2), operations['*'](8, 2));
        Assert.AreEqual<int>(Calculator.Divide(8, 2), operations['/'](8, 2));
    }

    [TestMethod]
    [DataRow("3 + 4", 7)]
    [DataRow("42 - 2", 40)]
    [DataRow("6 * 7", 42)]
    [DataRow("10 / 2", 5)]
    [DataRow("-3 + 4", 1)]
    public void TryCalculateValidExpressionReturnsTrueAndResult(string calculation, int expected)
    {
        bool success = new Calculator().TryCalculate(calculation, out int result);

        Assert.AreEqual<bool>(true, success);
        Assert.AreEqual<int>(expected, result);
    }

    [TestMethod]
    [DataRow("3+4")]
    [DataRow("3 +4")]
    [DataRow("3+ 4")]
    [DataRow("3  +  4")]
    [DataRow("a + 4")]
    [DataRow("3 + b")]
    [DataRow("3.5 + 4")]
    [DataRow("3 ^ 4")]
    [DataRow("3 ++ 4")]
    [DataRow("3 + 4 + 5")]
    [DataRow("3 / 0")]
    [DataRow("")]
    [DataRow(null)]
    public void TryCalculateInvalidExpressionReturnsFalse(string? calculation)
    {
        bool success = new Calculator().TryCalculate(calculation, out int result);

        Assert.AreEqual<bool>(false, success);
        Assert.AreEqual<int>(0, result);
    }
}
