namespace Calculate.Tests;

[TestClass]
public class CalculateTests
{
    [TestMethod]
    public void AddReturnsSum() => Assert.AreEqual<int>(7, Calculator.Add(3, 4));

    [TestMethod]
    public void SubtractReturnsDifference() => Assert.AreEqual<int>(40, Calculator.Subtract(42, 2));

    [TestMethod]
    public void SubtractCanReturnNegative() => Assert.AreEqual<int>(-1, Calculator.Subtract(3, 4));

    [TestMethod]
    public void AddHandlesNegativeOperands() => Assert.AreEqual<int>(-7, Calculator.Add(-3, -4));

    [TestMethod]
    public void MultiplyReturnsProduct() => Assert.AreEqual<int>(12, Calculator.Multiply(3, 4));

    [TestMethod]
    public void DivideReturnsQuotient() => Assert.AreEqual<int>(5, Calculator.Divide(10, 2));

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

    /*[TestMethod]
    [DataRow("2147483647 + 1")]
    [DataRow("-2147483648 - 1")]
    [DataRow("2147483647 * 2")]
    [DataRow("-2147483648 / -1")]
    public void TryCalculateOverflowThrows(string calculation)
    {
        Calculator calculator = new();
        Assert.ThrowsExactly<OverflowException>(() => calculator.TryCalculate(calculation, out _));
    }*/

    [TestMethod]
    public void DivideByZeroThrowsWithUndefinedMessage()
    {
        DivideByZeroException exception = Assert.ThrowsExactly<DivideByZeroException>(static () => Calculator.Divide(4, 0));
        Assert.AreEqual<string>("Division by zero is undefined.", exception.Message);
    }

    [TestMethod]
    public void DivideByZeroThrowsForIntToo() =>
        Assert.ThrowsExactly<DivideByZeroException>(static () => Calculator.Divide(4, 0));

    [TestMethod]
    public void TryCalculateDivisionByZeroThrows()
    {
        Calculator calculator = new();
        Assert.IsFalse((calculator.TryCalculate("4 / 0", out _)));
    }

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

    [TestMethod]
    [DataRow("3 + 4", 7)]
    [DataRow("42 - 2", 40)]
    [DataRow("6 * 7", 42)]
    [DataRow("9 / 3", 3)]
    [DataRow("-5 + 2", -3)]
    [DataRow("3 - 4", -1)]
    [DataRow("-3 - 4", -7)]
    [DataRow("3 - -4", 7)]
    [DataRow("1 + -4", -3)]
    [DataRow("-2 * -3", 6)]
    [DataRow("-6 / 3", -2)]
    public void TryCalculateSucceedsForValidExpressions(string calculation, int expected)
    {
        Calculator calculator = new();
        Assert.IsTrue(calculator.TryCalculate(calculation, out int result));
        Assert.AreEqual<int>(expected, result);
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