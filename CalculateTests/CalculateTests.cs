namespace Calculate.Tests;

[TestClass]
public sealed class CalculateTests
{
    // Calculator Tests --> Basic Operations

    [TestMethod]
    public void AddReturnsCorrectValue()
    {
        int result = Calculator.Add(3, 4);

        Assert.AreEqual<int>(7, result);
    }

    [TestMethod]
    public void AddHandlesNegativeOperands() => Assert.AreEqual<int>(-7, Calculator.Add(-3, -4));

    [TestMethod]
    public void SubtractReturnsCorrectValue()
    {
        int resulter = Calculator.Subtract(7, 2);
        Assert.AreEqual<int>(5, resulter);
    }

    [TestMethod]
    public void SubtractReturnsDifference() => Assert.AreEqual<int>(40, Calculator.Subtract(42, 2));

    [TestMethod]
    public void SubtractCanReturnNegative() => Assert.AreEqual<int>(-1, Calculator.Subtract(3, 4));

    [TestMethod]
    public void MultiplyMultipliesCorrectly()
    {
        int multimouser = Calculator.Multiply(3, 4);
        Assert.AreEqual<int>(12, multimouser);
    }

    [TestMethod]
    public void DivideDividesCorrectly()
    {
        int dividends = Calculator.Divide(24, 3);
        Assert.AreEqual<int>(8, dividends);
    }

    [TestMethod]
    public void DivideReturnsQuotient() => Assert.AreEqual<int>(5, Calculator.Divide(10, 2));

    [TestMethod]
    public void DivideRoundsOddValueDivision()
    {
        int dividends = Calculator.Divide(15, 2);
        Assert.AreEqual<int>(8, dividends);
    }

    //Overflow and divide by zero Tests 

    [TestMethod]
    public void AddOverflowThrows() =>
        Assert.ThrowsExactly<OverflowException>(static () => Calculator.Add(int.MaxValue, 1));

    [TestMethod]
    public void SubtractOverflowThrows() =>
        Assert.ThrowsExactly<OverflowException>(static () => Calculator.Subtract(int.MinValue, 1));

    [TestMethod]
    public void MultiplyOverflowThrows() =>
        Assert.ThrowsExactly<OverflowException>(static () => Calculator.Multiply(int.MaxValue, 2));

    [TestMethod]
    public void DivideOverflowThrows() =>
        Assert.ThrowsExactly<OverflowException>(static () => Calculator.Divide(int.MinValue, -1));

    [TestMethod]
    public void DivideByZeroThrowsWithUndefinedMessage()
    {
        DivideByZeroException exception = Assert.ThrowsExactly<DivideByZeroException>(static () => Calculator.Divide(4, 0));
        Assert.AreEqual<string>("Division by zero is undefined.", exception.Message);
    }

    // ---- MathematicalOperations ----

    [TestMethod]
    public void MathematicalOperationsContainsAllFourOperators()
    {
        Calculator calculator = new();
        Assert.HasCount(4, calculator.MathematicalOperations);
        Assert.AreEqual<int>(7, calculator.MathematicalOperations['+'](3, 4));
        Assert.AreEqual<int>(-1, calculator.MathematicalOperations['-'](3, 4));
        Assert.AreEqual<int>(12, calculator.MathematicalOperations['*'](3, 4));
        Assert.AreEqual<int>(2, calculator.MathematicalOperations['/'](8, 4));
    }

    // ---- TryCalculate ----

    [TestMethod]
    [DataRow("3 + 4", 7)]
    [DataRow("12 - 2", 10)]
    [DataRow("6 * 7", 42)]
    [DataRow("9 / 3", 3)]
    [DataRow("-5 + 2", -3)]
    [DataRow("3 - 4", -1)]
    [DataRow("-3 - 4", -7)]
    [DataRow("3 - -4", 7)]
    [DataRow("1 + -4", -3)]
    [DataRow("-2 * -5", 10)]
    [DataRow("-6 / 3", -2)]
    public void TryCalculateSucceedsForValidExpressions(string calculation, int expected)
    {
        Calculator calculator = new();
        Assert.IsTrue(calculator.TryCalculate(calculation, out int result));
        Assert.AreEqual<int>(expected, result);
    }

    [TestMethod]
    [DataRow("2147483647 + 1")]
    [DataRow("-2147483648 - 1")]
    [DataRow("2147483647 * 2")]
    [DataRow("-2147483648 / -1")]
    public void TryCalculateOverflowReturnsFalse(string calculation)
    {
        Calculator calculator = new();
        Assert.IsFalse(calculator.TryCalculate(calculation, out int result));
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculateDivisionByZeroReturnsFalse()
    {
        Calculator calculator = new();
        Assert.IsFalse(calculator.TryCalculate("4 / 0", out int result));
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    [DataRow("")]
    [DataRow("3+4")]
    [DataRow("3 +4")]
    [DataRow("3  + 4")]
    [DataRow("3 + 4 + 5")]
    [DataRow("a + 4")]
    [DataRow("3 + 1.5")]
    [DataRow("3 % 4")]
    [DataRow("3 ++ 4")]
    [DataRow("--3 + 4")]
    [DataRow("3 - - 4")]
    [DataRow("+ 3 4")]
    public void TryCalculateFailsForInvalidExpressions(string calculation)
    {
        Calculator calculator = new();
        Assert.IsFalse(calculator.TryCalculate(calculation, out int result));
        Assert.AreEqual<int>(0, result);
    }
}
