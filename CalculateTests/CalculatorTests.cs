using System.Reflection;

namespace Calculate;

public class CalculatorTests
{
    [Fact]
    public void TryCalculate_ValidAddition_ReturnsTrueAndCorrectAnswer()
    {
        //Arrange
        Calculator calc = new();

        //Act
        bool result = calc.TryCalculate("3 + 4", out double answer);

        //Assert
        Assert.True(result);
        Assert.Equal<double>(7.0, answer);
    }

    [Fact]
    public void TryCalculate_InvalidInput_ReturnsFalse()
    {
        //Arrange
        Calculator calc = new();

        //Act
        bool result = calc.TryCalculate("3+4", out double answer);
        bool result2 = calc.TryCalculate("  3 +  4", out double answer2);
        bool result3 = calc.TryCalculate("3-4", out double answer3);
        bool result4 = calc.TryCalculate("3*4", out double answer4);
        bool result5 = calc.TryCalculate("3/4", out double answer5);

        //Assert
        Assert.False(result);
        Assert.False(result2);
        Assert.False(result3);
        Assert.False(result4);
        Assert.False(result5);
    }
    [Fact]
    public void TryCalculate_ValidSubtraction_ReturnsTrueAndCorrectAnswer()
    {
        //Arrange
        Calculator calc = new();

        //Act
        bool result = calc.TryCalculate("6 - 4", out double answer);

        //Assert
        Assert.True(result);
        Assert.Equal<double>(2.0, answer);
    }
    [Fact]
    public void TryCalculate_ValidMultiplication_ReturnsTrueAndCorrectAnswer()
    {
        //Arrange
        Calculator calc = new();

        //Act
        bool result = calc.TryCalculate("2 * 2", out double answer);

        //Assert
        Assert.True(result);
        Assert.Equal<double>(4.0, answer);
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(5, 7, 12)]
    [InlineData(-3, 3, 0)]
    [InlineData(-4, -6, -10)]
    [InlineData(int.MaxValue, 0, int.MaxValue)]
    public void Add_ValidInputs_ReturnsCorrectSum(int a, int b, double expected)
    {
        // Arrange

        // Act
        double result = Calculator.Add(a, b);

        // Assert
        Assert.Equal<double>(expected, result);
    }

    [Fact]
    public void Add_AddToMaxInt_ThrowsOverflowException()
    {
        // Arrange
        int a = int.MaxValue;
        int b = 1;

        // Act & Assert
        Assert.Throws<OverflowException>(() => Calculator.Add(a, b));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(10, 4, 6)]
    [InlineData(-3, 3, -6)]
    [InlineData(-4, -6, 2)]
    [InlineData(int.MinValue, 0, int.MinValue)]
    public void Subtract_ValidInputs_ReturnsCorrectDifference(int a, int b, double expected)
    {
        // Arrange

        // Act
        double result = Calculator.Subtract(a, b);

        // Assert
        Assert.Equal<double>(expected, result);
    }

    [Fact]
    public void Subtract_SubtractFromMinInt_ThrowsOverflowException()
    {
        // Arrange
        int a = int.MinValue;
        int b = 1;

        // Act & Assert
        Assert.Throws<OverflowException>(() => Calculator.Subtract(a, b));
    }

    [Theory]
    [InlineData(0, 0, 0)]
    [InlineData(3, 4, 12)]
    [InlineData(-2, 3, -6)]
    [InlineData(-4, -5, 20)]
    [InlineData(int.MaxValue, 1, (double)int.MaxValue)]
    public void Multiply_ValidInputs_ReturnsCorrectProduct(int a, int b, double expected)
    {
        // Arrange

        // Act
        double result = Calculator.Multiply(a, b);

        // Assert
        Assert.Equal<double>(expected, result);
    }

    [Fact]
    public void Multiply_MultiplyMaxIntBy2_ThrowsOverflowException()
    {
        // Arrange
        int a = int.MaxValue;
        int b = 2;

        // Act & Assert
        Assert.Throws<OverflowException>(() => Calculator.Multiply(a, b));
    }

    [Theory]
    [InlineData(4, 2, 2.0)]
    [InlineData(5, 2, 2.5)]
    [InlineData(-6, 3, -2.0)]
    [InlineData(-8, -4, 2.0)]
    [InlineData(0, 5, 0.0)]
    [InlineData(int.MinValue, 1, (double)int.MinValue)]
    public void Divide_ValidInputs_ReturnsCorrectQuotient(int a, int b, double expected)
    {
        // Arrange

        // Act
        double result = Calculator.Divide(a, b);
        // Assert
        Assert.Equal<double>(expected, result);
    }
    [Fact]
    public void Divide_DividingByZero_ThrowsDivideByZeroException()
    {
        // Arrange
        int numerator = 2;
        int denominator = 0;
        // Act & Assert
        Assert.Throws<DivideByZeroException>(() => Calculator.Divide(numerator, denominator));
    }
}


