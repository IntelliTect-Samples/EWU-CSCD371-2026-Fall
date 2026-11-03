using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Tests;

[TestClass]
public sealed class CalculatorTests
{
    [TestMethod]
    public void ArithmeticMethods_ReturnCorrectResults()
    {
        // Arrange, Act, Assert
        Assert.AreEqual<int>(7, Calculator.Add(3, 4));
        Assert.AreEqual<int>(-1, Calculator.Subtract(3, 4));
        Assert.AreEqual<int>(12, Calculator.Multiply(3, 4));
        Assert.AreEqual<int>(2, Calculator.Divide(8, 4));
    }

    [TestMethod]
    public void MathematicalOperations_UsesCorrectMethods()
    {
        // Arrange
        Calculator calculator = new();
        var operations = calculator.MathematicalOperations;

        // Act and Assert
        Assert.HasCount(4, operations);
        Assert.AreEqual<int>(7, operations['+'](3, 4));
        Assert.AreEqual<int>(2, operations['-'](6, 4));
        Assert.AreEqual<int>(12, operations['*'](3, 4));
        Assert.AreEqual<int>(2, operations['/'](8, 4));
    }

    [TestMethod]
    [DataRow("3 + 4", 7)]
    [DataRow("42 - 2", 40)]
    [DataRow("5 * 6", 30)]
    [DataRow("12 / 3", 4)]
    [DataRow("-3 + 7", 4)]
    public void TryCalculate_ValidInput_ReturnsCorrectResult(
        string input, int expected)
    {
        // Arrange
        Calculator calculator = new();

        // Act
        bool success = calculator.TryCalculate(
            input, out int result);

        // Assert
        Assert.IsTrue(success);
        Assert.AreEqual<int>(expected, result);
    }

    [TestMethod]
    [DataRow("3+4")]
    [DataRow("3 +4")]
    [DataRow("3+ 4")]
    [DataRow("3 ^ 4")]
    [DataRow("abc + 2")]
    [DataRow("1 + 2 + 3")]
    [DataRow("8 / 0")]
    [DataRow("")]
    public void TryCalculate_InvalidInput_ReturnsFalse(
        string input)
    {
        // Arrange
        Calculator calculator = new();

        // Act
        bool success = calculator.TryCalculate(
            input, out int result);

        // Assert
        Assert.IsFalse(success);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculate_NullInput_ReturnsFalse()
    {
        Calculator calculator = new();

        bool success = calculator.TryCalculate(
            null, out int result);

        Assert.IsFalse(success);
        Assert.AreEqual<int>(0, result);
    }
}