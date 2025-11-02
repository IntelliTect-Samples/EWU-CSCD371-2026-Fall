using Calculate;
namespace CalculateTests;

[TestClass]
public sealed class CalculatorTests
{
    #region Overflow / Divide by zero checking

    [TestMethod]
    public void Add_MaxValue_ThrowsException() => //Arrange & Act & Assert
        Assert.ThrowsException<OverflowException>(() => Calculator.Add(int.MaxValue, int.MaxValue));

    [TestMethod]
    public void Subtract_MaxValue_ThrowsException() => //Arrange & Act & Assert
    Assert.ThrowsException<OverflowException>(() => Calculator.Subtract(int.MaxValue, -1));

    [TestMethod]
    public void Multiply_MaxValue_ThrowsException() => //Arrange & Act & Assert
        Assert.ThrowsException<OverflowException>(() => Calculator.Multiply(int.MaxValue, int.MaxValue));

    [TestMethod]
    public void Divide_ByZero_ThrowsException() =>//Arrange & Act & Assert
        Assert.ThrowsException<DivideByZeroException>(() => Calculator.Divide(int.MaxValue, 0));

    #endregion
    [TestMethod]
    [DataRow("2*2")]
    [DataRow("2+2")]
    [DataRow("2-2")]
    [DataRow("2/2")]
    public void TryParse_OperationNoSpaces_InvalidString(string input)
    {
        Assert.IsFalse(Calculator.TryCalculate(input, out int result));
    }

    [TestMethod]
    [DataRow("2 * 2", 4)]
    [DataRow("2 + 2", 4)]
    [DataRow("2 - 2", 0)]
    [DataRow("2 / 2", 1)]
    public void TryParse_OperationSpaces_Success(string input, int expected)
    {
        Assert.IsTrue(Calculator.TryCalculate(input, out int result));
        Assert.AreEqual<int>(result, expected);
    }

    [TestMethod]
    [DataRow("2147483647 * 2147483647")]
    [DataRow("2147483647 + 2147483647")]
    [DataRow("2147483647 - -1")]
    public void TryParse_Overflow_ReturnsFalse(string input)
    {
        Assert.IsFalse(Calculator.TryCalculate(input, out int result));
    }

    [TestMethod]
    [DataRow("2 / 0")]
    public void TryParse_DivideByZero_ReturnsFalse(string input)
    {
        Assert.IsFalse(Calculator.TryCalculate(input, out int result));
    }

    [TestMethod]
    [DataRow("2.0 / 5.0")]
    [DataRow("3.14159265359 / 2.71828")]
    public void TryParse_NonInts_ReturnsFalse(string input)
    {
        Assert.IsFalse(Calculator.TryCalculate(input, out int result));
    }
}
