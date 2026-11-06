using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculator.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void Add_ReturnsSum() => Assert.AreEqual<int>(7, Calculator.Add(3, 4));

    [TestMethod]
    public void Subtract_ReturnsDifference() => Assert.AreEqual<int>(40, Calculator.Subtract(42, 2));

    [TestMethod]
    public void Multiply_ReturnsProduct() => Assert.AreEqual<int>(12, Calculator.Multiply(3, 4));

    [TestMethod]
    public void Divide_ReturnsQuotient() => Assert.AreEqual<int>(5, Calculator.Divide(10, 2));

    [TestMethod]
    public void MathematicalOperations_ContainsTheFourOperators()
    {
        IReadOnlyDictionary<char, Func<int, int, int>> operations = new Calculator().MathematicalOperations;

        Assert.AreEqual<int>(4, operations.Count);
        foreach (char op in "+-*/")
        {
            Assert.AreEqual<bool>(true, operations.ContainsKey(op));
        }
    }

    [TestMethod]
    public void MathematicalOperations_MapToTheMatchingMethods()
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
    public void TryCalculate_ValidExpression_ReturnsTrueAndResult(string calculation, int expected)
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
    public void TryCalculate_InvalidExpression_ReturnsFalse(string? calculation)
    {
        bool success = new Calculator().TryCalculate(calculation, out int result);

        Assert.AreEqual<bool>(false, success);
        Assert.AreEqual<int>(0, result);
    }
}
