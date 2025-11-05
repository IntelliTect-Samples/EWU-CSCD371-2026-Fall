using Calculate;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;


namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{

    [TestMethod]
    public void Add_TwoPositiveIntegers_ReturnsSum()
    {
        // Arrange
        int a = 2;
        int b = 8;

        // Act
        int result = Calculator.Add(a, b);

        // Assert
        Assert.AreEqual<int>(10, result);
    }

    [TestMethod]
    public void Subtract_NegativeResult_ReturnsDifference()
    {
        // Arrange
        int a = 5;
        int b = 9;

        // Act
        int result = Calculator.Subtract(a, b);

        // Assert
        Assert.AreEqual<int>(-4, result);
    }

    [TestMethod]
    public void Multiply_WithZero_ReturnsZero()
    {
        // Arrange
        int a = 20;
        int b = 0;

        // Act
        int result = Calculator.Multiply(a, b);

        // Assert
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void Divide_WithEvenDivision_ReturnsQuotient()
    {
        // Arrange
        int a = 10;
        int b = 5;

        // Act
        int result = Calculator.Divide(a, b);

        // Assert
        Assert.AreEqual<int>(2, result);
    }

    [TestMethod]
    [ExpectedException(typeof(DivideByZeroException))]
    public void Divide_ByZero_ThrowsDivideByZeroException()
    {
        // Arrange
        int a = 12;
        int b = 0;

        // Act
        _ = Calculator.Divide(a, b);

        // Assert (handled by ExpectedException)
    }

    [TestMethod]
    public void MathematicalOperations_HasFourOperators_MapsToExpectedMethods()
    {
        // Arrange
        IReadOnlyDictionary<char, Func<int, int, int>> map = new Calculator().MathematicalOperations;

        // Act / Assert
        Assert.AreEqual<int>(4, map.Count);
        Assert.AreEqual<bool>(true, map.ContainsKey('+'));
        Assert.AreEqual<bool>(true, map.ContainsKey('-'));
        Assert.AreEqual<bool>(true, map.ContainsKey('*'));
        Assert.AreEqual<bool>(true, map.ContainsKey('/'));
    }

    [TestMethod]
    public void MathematicalOperations_InvokeAdd_PerformsAddition()
    {
        // Arrange
        Func<int, int, int> add = new Calculator().MathematicalOperations['+'];

        // Act
        int result = add(6, 14);

        // Assert
        Assert.AreEqual<int>(20, result);
    }

    [TestMethod]
    public void MathematicalOperations_InvokeEachOperator_PerformsCorrectOperation()
    {
        // Arrange
        IReadOnlyDictionary<char, Func<int, int, int>> map = new Calculator().MathematicalOperations;

        // Act
        int sum = map['+'](2, 9);
        int diff = map['-'](15, 7);
        int prod = map['*'](2, 5);
        int quot = map['/'](21, 7);

        // Assert
        Assert.AreEqual<int>(11, sum);
        Assert.AreEqual<int>(8, diff);
        Assert.AreEqual<int>(10, prod);
        Assert.AreEqual<int>(3, quot);
    }

    [TestMethod]
    public void TryCalculate_NegativeNumbers_ReturnsTrue()
    {
        // Arrange
        Calculator calc = new Calculator();
        string input = "-8 - -3";

        // Act
        bool ok = calc.TryCalculate(input, out int result);

        // Assert
        Assert.AreEqual<bool>(true, ok);
    }

    [TestMethod]
    public void TryCalculate_NegativeNumbers_ReturnsCorrectResult()
    {
        // Arrange
        Calculator calc = new Calculator();
        string input = "-8 - -3";

        // Act
        bool ok = calc.TryCalculate(input, out int result);

        // Assert
        Assert.AreEqual<int>(-5, result);
    }

    [TestMethod]
    public void TryCalculate_ValidAddition_WithSpaces_ReturnsTrueAndSum()
    {
        // Arrange
        Calculator calc = new Calculator();
        string input = "3 + 4";

        // Act
        bool ok = calc.TryCalculate(input, out int result);

        // Assert
        Assert.AreEqual<bool>(true, ok);
        Assert.AreEqual<int>(7, result);
    }

    [TestMethod]
    public void TryCalculate_NoSpacesAroundOperator_ReturnsFalseAndDefault()
    {
        // Arrange
        Calculator calc = new Calculator();
        string input = "3+4";

        // Act
        bool ok = calc.TryCalculate(input, out int result);

        // Assert
        Assert.AreEqual<bool>(false, ok);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculate_NonIntegerOperands_ReturnsFalseAndDefault()
    {
        // Arrange
        Calculator calc = new Calculator();
        string input = "x + y";

        // Act
        bool ok = calc.TryCalculate(input, out int result);

        // Assert
        Assert.AreEqual<bool>(false, ok);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculate_UnknownOperator_ReturnsFalseAndDefault()
    {
        // Arrange
        Calculator calc = new Calculator();
        string input = "3 ^ 4";

        // Act
        bool ok = calc.TryCalculate(input, out int result);

        // Assert
        Assert.AreEqual<bool>(false, ok);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculate_DivideByZero_ReturnsFalseAndDefault()
    {
        // Arrange
        Calculator calc = new Calculator();
        string input = "10 / 0";

        // Act
        bool ok = calc.TryCalculate(input, out int result);

        // Assert
        Assert.AreEqual<bool>(false, ok);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void GenericCalculator_DoubleAdds_Success()
    {
        var calc = new Calculate.Calculator<double>();
        Assert.IsTrue(calc.TryCalculate("5.5 + 4.5", out var result));
        Assert.AreEqual<double>(10.0, result);
    }

    [TestMethod]
    public void GenericCalculator_DecimalMultiplies_Success()
    {
        var calc = new Calculate.Calculator<decimal>();
        Assert.IsTrue(calc.TryCalculate("2.5 * 4.0", out decimal result));
        Assert.AreEqual<decimal>(10.0m, result);
    }

    [TestMethod]
    public void GenericCalculator_FloatDivides_Success()
    {
        var calc = new Calculate.Calculator<float>();
        Assert.IsTrue(calc.TryCalculate("9.0 / 3.0", out float result));
        Assert.AreEqual<float>(3.0f, result);
    }

    [TestMethod]
    public void GenericCalculator_LongSubtracts_Success()
    {
        var calc = new Calculate.Calculator<long>();
        Assert.IsTrue(calc.TryCalculate("20 - 7", out long result));
        Assert.AreEqual<long>(13, result);
    }

    [TestMethod]
    public void GenericCalculator_DivideByZero_Fails()
    {
        var calc = new Calculate.Calculator<int>();
        Assert.IsFalse(calc.TryCalculate("5.0 / 0.0", out var result));
    }
}