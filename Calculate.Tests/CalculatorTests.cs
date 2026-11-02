using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    [DataRow(3, 4, 7L)]
    [DataRow(-3, 3, 0L)]
    [DataRow(int.MaxValue, int.MaxValue, 4294967294L)]
    public void Add_IntegerOperands_ReturnsSum(int left, int right, long expected)
    {
        Assert.AreEqual<decimal>(expected, Calculator.Add(left, right));
    }

    [TestMethod]
    [DataRow(42, 2, 40L)]
    [DataRow(2, 5, -3L)]
    [DataRow(int.MinValue, int.MaxValue, -4294967295L)]
    public void Subtract_IntegerOperands_ReturnsDifference(int left, int right, long expected)
    {
        Assert.AreEqual<decimal>(expected, Calculator.Subtract(left, right));
    }

    [TestMethod]
    [DataRow(-3, 4, -12L)]
    [DataRow(0, 4, 0L)]
    [DataRow(int.MinValue, int.MinValue, 4611686018427387904L)]
    public void Multiply_IntegerOperands_ReturnsProduct(int left, int right, long expected)
    {
        Assert.AreEqual<decimal>(expected, Calculator.Multiply(left, right));
    }

    [TestMethod]
    public void Divide_UnevenOperands_PreservesFraction()
    {
        Assert.AreEqual<decimal>(3.5M, Calculator.Divide(7, 2));
    }

    [TestMethod]
    [DataRow(-12, 4, -3L)]
    [DataRow(0, 4, 0L)]
    [DataRow(int.MinValue, -1, 2147483648L)]
    public void Divide_IntegerOperands_ReturnsQuotient(int left, int right, long expected)
    {
        Assert.AreEqual<decimal>(expected, Calculator.Divide(left, right));
    }

    [TestMethod]
    public void Divide_ZeroDivisor_ThrowsDivideByZeroException()
    {
        Assert.ThrowsExactly<DivideByZeroException>(() => Calculator.Divide(3, 0));
    }

    [TestMethod]
    [DataRow('+', 16)]
    [DataRow('-', 8)]
    [DataRow('*', 48)]
    [DataRow('/', 3)]
    public void MathematicalOperations_Operator_ReferencesExpectedMethod(char operation, int expected)
    {
        Calculator calculator = new();
        decimal result = calculator.MathematicalOperations[operation](12, 4);

        Assert.HasCount(4, calculator.MathematicalOperations);
        Assert.AreEqual<decimal>(expected, result);
    }

    [TestMethod]
    [DataRow("3 + 4", 7L)]
    [DataRow("42 - 2", 40L)]
    [DataRow("-3 * 4", -12L)]
    [DataRow("12 / -4", -3L)]
    [DataRow("0 / 4", 0L)]
    [DataRow(" +3  +  +4 ", 7L)]
    [DataRow("\t3\t+\t4\t", 7L)]
    [DataRow("2147483647 + 2147483647", 4294967294L)]
    [DataRow("-2147483648 / -1", 2147483648L)]
    public void TryCalculate_ValidExpression_ReturnsTrueAndResult(string calculation, long expected)
    {
        Calculator calculator = new();

        bool success = calculator.TryCalculate(calculation, out decimal result);

        Assert.IsTrue(success);
        Assert.AreEqual<decimal>(expected, result);
    }

    [TestMethod]
    public void TryCalculate_UnevenDivision_ReturnsFraction()
    {
        Calculator calculator = new();

        Assert.IsTrue(calculator.TryCalculate("7 / 2", out decimal result));
        Assert.AreEqual<decimal>(3.5M, result);
    }

    [TestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow(" \t ")]
    [DataRow("3")]
    [DataRow("3 +")]
    [DataRow("3 + 4 extra")]
    [DataRow("3 + 4 + 5")]
    [DataRow("3+4")]
    [DataRow("3+ 4")]
    [DataRow("3 +4")]
    [DataRow("3 ++ 4")]
    [DataRow("3 % 4")]
    [DataRow("three + 4")]
    [DataRow("3 + four")]
    [DataRow("3.5 + 4")]
    [DataRow("3 + 4.5")]
    [DataRow("2147483648 + 1")]
    [DataRow("1 + -2147483649")]
    [DataRow("3 / 0")]
    public void TryCalculate_InvalidExpression_ReturnsFalseAndZero(string? calculation)
    {
        Calculator calculator = new();

        bool success = calculator.TryCalculate(calculation, out decimal result);

        Assert.IsFalse(success);
        Assert.AreEqual<decimal>(0, result);
    }
}
