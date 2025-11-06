namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void MathematicalOperations_ExposesFourOperators_BehaveCorrectly()
    {
        var ops = new Calculator().MathematicalOperations;

        Assert.AreEqual(4, ops.Count, "Expected exactly four operators.");
        CollectionAssert.AreEquivalent(new List<char> { '+', '-', '*', '/' }, new List<char>(ops.Keys));

        Assert.AreEqual<int>(5, ops['+'](2, 3));
        Assert.AreEqual<int>(-1, ops['-'](2, 3));
        Assert.AreEqual<int>(6, ops['*'](2, 3));
        Assert.AreEqual<int>(2, ops['/'](6, 3));
    }

    [TestMethod]
    public void BasicArithmeticMethods_ValidIntInputs_ExpectedResults()
    {
        Assert.AreEqual<int>(9, Calculator.Add(4, 5));
        Assert.AreEqual<int>(-1, Calculator.Subtract(4, 5));
        Assert.AreEqual<int>(20, Calculator.Multiply(4, 5));
        Assert.AreEqual<int>(2, Calculator.Divide(10, 5));
    }

    [DataTestMethod]
    [DataRow("2 + 3", 5)]
    [DataRow("2 + 3 * 4", 14)]                 // precedence
    [DataRow("10 + 2 * 6 - 4 / 2", 20)]        // mixed precedence and associativity
    [DataRow("8 / 4 + 1", 3)]
    [DataRow("  7   -   2   ", 5)]             // excessive spaces
    [DataRow("-5 + 3", -2)]                    // leading negative number
    [DataRow("5 * -3", -15)]                   // negative operand after operator
    [DataRow("-5", -5)]                        // single negative literal
    public void TryCalculate_ValidExpressions_ReturnTrueAndCorrectResult(string input, int expected)
    {
        Calculator c = new();
        bool ok = c.TryCalculate(input, out var result);

        Assert.IsTrue(ok, $"Expected success for '{input}'.");
        Assert.AreEqual<int>(expected, result);
    }

    [TestMethod]
    public void TryCalculate_DivideByZero_ReturnsFalse()
    {
        Calculator c = new();
        bool ok = c.TryCalculate("1 / 0", out var result);

        Assert.IsFalse(ok);
        Assert.AreEqual<int>(0, result);
    }

    [DataTestMethod]
    [DataRow(null)]
    [DataRow("")]
    [DataRow("   ")]
    public void TryCalculate_NullOrWhitespace_ReturnsFalse(string input)
    {
        Calculator c = new();
        bool ok = c.TryCalculate(input, out var result);

        Assert.IsFalse(ok);
        Assert.AreEqual<int>(0, result);
    }

    [DataTestMethod]
    [DataRow("2 ^ 3")]           // unsupported operator
    [DataRow("foo")]             // not a number or operator
    [DataRow("2.5 + 3")]         // non-integer token
    [DataRow("2 + ( 3 )")]       // parentheses not supported
    public void TryCalculate_InvalidTokens_ReturnsFalse(string input)
    {
        Calculator c = new();

        bool ok = c.TryCalculate(input, out var result);

        Assert.IsFalse(ok);
        Assert.AreEqual<int>(0, result);
    }

    [DataTestMethod]
    [DataRow("2 + + 3")]         // collapses to malformed postfix
    [DataRow("2 +")]             // operator without rhs
    [DataRow("+ 2")]             // starts with operator; postfix will fail
    [DataRow("2 3")]             // extra operand left over
    public void TryCalculate_MalformedExpressions_ReturnsFalse(string input)
    {
        Calculator c = new();

        bool ok = c.TryCalculate(input, out var result);

        Assert.IsFalse(ok);
        Assert.AreEqual<int>(0, result);
    }

    [TestMethod]
    public void TryCalculate_UnknownOperator_ReturnsFalse()
    {
        Calculator c = new();

        bool ok = c.TryCalculate("5 % 2", out var result);

        Assert.IsFalse(ok);
        Assert.AreEqual<int>(0, result);
    }
}
