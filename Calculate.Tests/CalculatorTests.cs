namespace Calculate.Tests;

[TestClass]
public sealed class CalculatorTests
{
    [TestMethod]
    public void Add_TwoNumbers_ReturnsSum() => Assert.AreEqual<int>(7, Calculator.Add(3, 4));

    [TestMethod]
    public void Subtract_TwoNumbers_ReturnsDifference() => Assert.AreEqual<int>(40, Calculator.Subtract(42, 2));

    [TestMethod]
    public void Multiply_TwoNumbers_ReturnsProduct() => Assert.AreEqual<int>(12, Calculator.Multiply(3, 4));

    [TestMethod]
    public void Divide_TwoNumbers_ReturnsQuotient() => Assert.AreEqual<int>(5, Calculator.Divide(10, 2));

    [TestMethod]
    public void MathematicalOperations_ContainsFourOperators()
    {
        Calculator calculator = new();

        Assert.AreEqual<int>(4, calculator.MathematicalOperations.Count);
        Assert.AreEqual<int>(7, calculator.MathematicalOperations['+'](3, 4));
        Assert.AreEqual<int>(-1, calculator.MathematicalOperations['-'](3, 4));
        Assert.AreEqual<int>(12, calculator.MathematicalOperations['*'](3, 4));
        Assert.AreEqual<int>(2, calculator.MathematicalOperations['/'](8, 4));
    }

    [TestMethod]
    [DataRow("3 + 4", 7)]
    [DataRow("42 - 2", 40)]
    [DataRow("6 * 7", 42)]
    [DataRow("10 / 2", 5)]
    [DataRow("-3 + 5", 2)]
    public void TryCalculate_ValidExpression_ReturnsTrueAndResult(string calculation, int expected)
    {
        Calculator calculator = new();

        Assert.IsTrue(calculator.TryCalculate(calculation, out int result));
        Assert.AreEqual<int>(expected, result);
    }

    [TestMethod]
    [DataRow("3+4")]
    [DataRow("3 +4")]
    [DataRow("3  + 4")]
    [DataRow("a + 4")]
    [DataRow("3 + b")]
    [DataRow("3.5 + 4")]
    [DataRow("3 % 4")]
    [DataRow("3 ++ 4")]
    [DataRow("3 + 4 + 5")]
    [DataRow("10 / 0")]
    [DataRow("")]
    [DataRow(null)]
    public void TryCalculate_InvalidExpression_ReturnsFalse(string? calculation)
    {
        Calculator calculator = new();

        Assert.IsFalse(calculator.TryCalculate(calculation, out int result));
        Assert.AreEqual<int>(0, result);
    }
}
