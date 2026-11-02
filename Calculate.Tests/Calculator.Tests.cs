namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void Add_TwoIntegers_ReturnsSum()
    {
        Assert.AreEqual<int>(7, Calculator.Add(3, 4));
    }

    [TestMethod]
    public void Subtract_TwoIntegers_ReturnsDifference()
    {
        Assert.AreEqual<int>(40, Calculator.Subtract(42, 2));
    }

    [TestMethod]
    public void Multiply_TwoIntegers_ReturnsProduct()
    {
        Assert.AreEqual<int>(12, Calculator.Multiply(3, 4));
    }

    [TestMethod]
    public void Divide_TwoIntegers_ReturnsQuotient()
    {
        Assert.AreEqual<int>(5, Calculator.Divide(10, 2));
    }

    [TestMethod]
    public void MathematicalOperations_ContainsExactlyFourOperators()
    {
        Calculator calculator = new();

        Assert.AreEqual<int>(4, calculator.MathematicalOperations.Count);
    }

    [TestMethod]
    [DataRow('+', 7)]
    [DataRow('-', -1)]
    [DataRow('*', 12)]
    [DataRow('/', 0)]
    public void MathematicalOperations_IndexByOperator_InvokesMatchingMethod(char @operator, int expected)
    {
        Calculator calculator = new();

        int result = calculator.MathematicalOperations[@operator](3, 4);

        Assert.AreEqual<int>(expected, result);
    }

    [TestMethod]
    [DataRow("3 + 4", 7)]
    [DataRow("42 - 2", 40)]
    [DataRow("6 * 7", 42)]
    [DataRow("20 / 5", 4)]
    [DataRow("-3 + 10", 7)]
    public void TryCalculate_ValidExpression_ReturnsTrueAndResult(string calculation, int expected)
    {
        Calculator calculator = new();

        bool success = calculator.TryCalculate(calculation, out int result);

        Assert.AreEqual<bool>(true, success);
        Assert.AreEqual<int>(expected, result);
    }

    [TestMethod]
    [DataRow("3+4")]          // no whitespace around operator
    [DataRow("3 +4")]         // missing whitespace on one side
    [DataRow("3 + four")]     // operand not an integer
    [DataRow("3.5 + 1")]      // operand not an integer
    [DataRow("3 % 4")]        // unsupported operator
    [DataRow("3 ++ 4")]       // operator longer than one char
    [DataRow("3 + 4 + 5")]    // too many parts
    [DataRow("3  + 4")]       // extra whitespace
    [DataRow("10 / 0")]       // divide by zero
    [DataRow("")]
    [DataRow(null)]
    public void TryCalculate_InvalidExpression_ReturnsFalse(string? calculation)
    {
        Calculator calculator = new();

        bool success = calculator.TryCalculate(calculation, out int result);

        Assert.AreEqual<bool>(false, success);
        Assert.AreEqual<int>(0, result);
    }
}