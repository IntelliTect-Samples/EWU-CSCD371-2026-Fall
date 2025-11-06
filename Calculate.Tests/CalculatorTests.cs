namespace Calculate.Tests;

[TestClass]
public sealed class CalculatorTests
{
    [TestMethod]
    public void MathematicalOperations_ContainsAllOperators_Success()
    {
        // Arrange
        var calculator = new Calculator();
        var expectedOperators = new[] { '+', '-', '*', '/' };

        // Act & Assert
        foreach (var op in expectedOperators)
        {
            Assert.IsTrue(calculator.MathematicalOperations.ContainsKey(op));
        }
    }

    [TestMethod]
    public void Add_ReturnsCorrectSum_Success()
    {
        // Arrange & Act
        var result = Calculator.Add(3, 4);

        // Assert
        Assert.AreEqual<int>(7, result);
    }

    [TestMethod]
    public void Subtract_ReturnsCorrectDifference_Success()
    {
        // Arrange & Act
        var result = Calculator.Subtract(10, 3);

        // Assert
        Assert.AreEqual<int>(7, result);
    }

    [TestMethod]
    public void Multiple_ReturnsCorrectProduct_Success()
    {
        // Arrange & Act
        var result = Calculator.Multiply(3, 4);

        // Assert
        Assert.AreEqual<int>(12, result);
    }

    [TestMethod]
    public void Divide_ReturnsCorrectQuotient_Success()
    {
        // Arrange & Act
        var result = Calculator.Divide(12, 3);

        // Assert
        Assert.AreEqual<int>(4, result);
    }

    [TestMethod]
    public void TryCalculate_ValidAddition_ReturnsTrue()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var success = calculator.TryCalculate("3 + 4", out int result);

        // Assert
        Assert.IsTrue(success);
        Assert.AreEqual<int>(7, result);
    }

    [TestMethod]
    public void TryCalculate_ValidSubtraction_ReturnsTrue()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var success = calculator.TryCalculate("10 - 3", out int result);

        // Assert
        Assert.IsTrue(success);
        Assert.AreEqual<int>(7, result);
    }

    [TestMethod]
    public void TryCalculate_ValidMultiplication_ReturnsTrue()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var success = calculator.TryCalculate("3 * 4", out int result);

        // Assert
        Assert.IsTrue(success);
        Assert.AreEqual<int>(12, result);
    }

    [TestMethod]
    public void TryCalculate_ValidDivision_ReturnsTrue()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var success = calculator.TryCalculate("12 / 3", out int result);

        // Assert
        Assert.IsTrue(success);
        Assert.AreEqual<int>(4, result);
    }

    [TestMethod]
    public void TryCalculate_NoSpaces_ReturnsFalse()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var success = calculator.TryCalculate("3+4", out int result);

        // Assert
        Assert.IsFalse(success);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculate_NonNumericOperands_ReturnsFalse()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var success = calculator.TryCalculate("a + b", out int result);

        // Assert
        Assert.IsFalse(success);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculate_MissingOperand_ReturnsFalse()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var success = calculator.TryCalculate("3 + ", out int result);

        // Assert
        Assert.IsFalse(success);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculate_InvalidOperator_ReturnsFalse()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var success = calculator.TryCalculate("3 $ 4", out int result);

        // Assert
        Assert.IsFalse(success);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculate_ExtraSpaces_ReturnsTrue()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var success = calculator.TryCalculate("  3  +  4", out int result);

        // Assert
        Assert.IsTrue(success);
        Assert.AreEqual<int>(7, result);
    }

    [TestMethod]
    public void TryCalculate_NegativeNumbers_ReturnsTrue()
    {
        // Arrange
        var calculator = new Calculator();

        // Act
        var success = calculator.TryCalculate("-5 + 3", out int result);

        // Assert
        Assert.IsTrue(success);
        Assert.AreEqual<int>(-2, result);
    }
}

