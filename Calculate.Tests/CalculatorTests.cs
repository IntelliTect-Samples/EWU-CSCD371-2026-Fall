namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{
    #region Addition Tests
    
    [TestMethod]
    public void Add_WithTwoIntegers_ReturnsSum()
    {
        const int left = 3;
        const int right = 4;
        
        int actual = Calculator.Add(left, right);
        
        Assert.AreEqual(7, actual);
    }
    
    [TestMethod]
    public void MathematicalOperations_WithAdditionOperator_ReturnsAdditionBehavior()
    {
        Calculator calculator = new();
        
        int actual = calculator.MathematicalOperations['+'](3, 4);
        
        Assert.AreEqual(7, actual);
    }
    
    [TestMethod]
    public void TryCalculate_WithAdditionExpression_ReturnsTrueAndResult()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate("3 + 4", out int result);
        
        Assert.IsTrue(actual);
        Assert.AreEqual(7, result);
    }
    
    #endregion
    
    #region Subtraction Tests

    [TestMethod]
    public void Subtract_WithTwoIntegers_ReturnsDifference()
    {
        const int left = 3;
        const int right = 4;
        
        int actual = Calculator.Subtract(left, right);
        
        Assert.AreEqual(-1, actual);
    }

    [TestMethod]
    public void MathematicalOperations_WithSubtractionOperator_ReturnsSubtractionBehavior()
    {
        Calculator calculator = new();
        
        int actual = calculator.MathematicalOperations['-'](3, 4);
        
        Assert.AreEqual(-1, actual);
    }

    [TestMethod]
    public void TryCalculate_WithSubtractionExpression_ReturnsTrueAndResult()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate("3 - 4", out int result);
        
        Assert.IsTrue(actual);
        Assert.AreEqual(-1, result);
    }

    #endregion
    
    #region Multiplication Tests

    [TestMethod]
    public void Multiply_WithTwoIntegers_ReturnsProduct()
    {
        const int left = 3;
        const int right = 4;
        
        int result = Calculator.Multiply(left, right);
        
        Assert.AreEqual(12, result);
    }

    [TestMethod]
    public void MathematicalOperations_WithMultiplicationOperator_ReturnsMultiplicationBehavior()
    {
        Calculator calculator = new();
        
        int actual = calculator.MathematicalOperations['*'](3, 4);
        
        Assert.AreEqual(12, actual);
    }

    [TestMethod]
    public void TryCalculate_WithMultiplicationExpression_ReturnsTrueAndResult()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate("3 * 4", out int result);
        
        Assert.IsTrue(actual);
        Assert.AreEqual(12, result);
    }

    #endregion
    
    #region Division Tests

    [TestMethod]
    public void Divide_WithTwoIntegers_ReturnsQuotient()
    {
        const int left = 8;
        const int right = 4;
        
        int actual = Calculator.Divide(left, right);
        
        Assert.AreEqual(2, actual);
    }

    [TestMethod]
    public void MathematicalOperations_WithDivisionOperator_ReturnsDivisionBehavior()
    {
        Calculator calculator = new();
        
        int actual = calculator.MathematicalOperations['/'](8, 4);
        
        Assert.AreEqual(2, actual);
    }

    [TestMethod]
    public void TryCalculate_WithDivisionExpression_ReturnsTrueAndResult()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate("8 / 4", out int result);
        
        Assert.IsTrue(actual);
        Assert.AreEqual(2, result);
    }
    
    #endregion
    
    #region DivideByZeroException Tests

    [TestMethod]
    public void Divide_DivideByZero_ThrowsDivideByZeroException()
    {
        const int left = 8;
        const int right = 0;
        
        Assert.ThrowsExactly<DivideByZeroException>(() => Calculator.Divide(left, right));
    }

    [TestMethod]
    public void MathematicalOperations_DivideByZero_ThrowsDivideByZeroException()
    {
        Calculator calculator = new();
        
        Assert.ThrowsExactly<DivideByZeroException>(() => calculator.MathematicalOperations['/'](8, 0));
    }

    [TestMethod]
    public void TryCalculate_DivideByZero_ReturnsFalse()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate("8 / 0", out int _);
        
        Assert.IsFalse(actual);
    }

    #endregion

    #region Try Calculate Invalid String Tests

    [TestMethod]
    public void TryCalculate_StringNotAnEquation_ReturnsFalse()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate("abc", out int _);
        
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void TryCalculate_TooManyWhiteSpaces_ReturnsFalse()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate(" 3 + 6 ", out int _);
        
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void TryCalculate_EquationWithoutWhiteSpaces_ReturnsFalse()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate("3+6", out int _);
        
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void TryCalculate_TooManyPartsGiven_ReturnsFalse()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate("3 + 6 - 7", out int _);
        
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void TryCalculate_InvalidOperatorGiven_ReturnsFalse()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate("3 % 6", out int _);
        
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void TryCalculate_NotGivenInts_ReturnsFalse()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate("A + B", out int _);
        
        Assert.IsFalse(actual);
    }

    [TestMethod]
    public void TryCalculate_NullString_ReturnsFalse()
    {
        Calculator calculator = new();
        
        bool actual = calculator.TryCalculate(null, out int _);
        
        Assert.IsFalse(actual);
    }

    #endregion
}