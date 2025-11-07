using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Calculate.Tests;

[TestClass]
public class CalculatorTests
{
    [TestMethod]
    public void TryCalculate_Addition_ReturnsExpectedResult()
    {
        bool ok = Calculator.TryCalculate<double>("3.3 + 4.7", out double result);
        Assert.IsTrue(ok);
        Assert.AreEqual<double>(8.0, result);
    }

    [TestMethod]
    public void TryCalculate_Subtraction_ReturnsExpectedResult()
    {
        bool ok = Calculator.TryCalculate<double>("10.5 - 7.5", out double result);
        Assert.IsTrue(ok);
        Assert.AreEqual<double>(3.0, result);
    }

    [TestMethod]
    public void TryCalculate_Multiplication_ReturnsExpectedResult()
    {
        bool ok = Calculator.TryCalculate<int>("2 * 7", out int result);
        Assert.IsTrue(ok);
        Assert.AreEqual<int>(14, result);
    }

    [TestMethod]
    public void TryCalculate_Division_ReturnsExpectedResult()
    {
        bool ok = Calculator.TryCalculate<int>("10 / 5", out int result);
        Assert.IsTrue(ok);
        Assert.AreEqual<int>(2, result);
    }

    [TestMethod]
    public void TryCalculate_NoSpacesAroundOperator_ReturnsFalse()
    {
        bool temp = Calculator.TryCalculate<int>("3+4", out int result);
        Assert.IsFalse(temp);
    }

    [TestMethod]
    public void TryCalculate_InvalidInput_ReturnsFalse()
    {
        bool temp = Calculator.TryCalculate<double>("abc + 2", out double result);
        Assert.IsFalse(temp);
    }

    [TestMethod]
    public void TryCalculate_DivideByZero_ReturnsFalse()
    {
        bool temp = Calculator.TryCalculate<int>("10 / 0", out int result);
        Assert.IsFalse(temp);
    }


    
}
