using System.Runtime.CompilerServices;

namespace Calculate.Tests;

[TestClass]
public sealed class CalculatorTests
{
    [TestMethod]
    public void Add_ReturnsCorrectValue()
    {
        int result = Calculator.Add(3, 4);

        Assert.AreEqual<int>(7, result);
    }
    [TestMethod]
    public void Subtract_ReturnsCorrectValue() { 
        int resulter = Calculator.Subtract(7, 2);
        Assert.AreEqual<int>(5, resulter);
    }
    [TestMethod]
    public void Multiply_MultipliesCorrectly() {
        int multimouser = Calculator.Multiply(3, 4);
        Assert.AreEqual<int>(12, multimouser);
    }
    [TestMethod]
    public void Divide_DividesCorrectly(){
        int dividends = Calculator.Divide(24, 3);
        Assert.AreEqual<int>(8, dividends);
    }
    [TestMethod]
    public void Divide_RoundsOddValueDivision() {
        int dividends = Calculator.Divide(15, 2);
        Assert.AreEqual(8, dividends);
    }
}
