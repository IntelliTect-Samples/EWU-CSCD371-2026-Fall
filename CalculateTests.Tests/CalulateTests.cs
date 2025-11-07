using Microsoft.VisualStudio.TestTools.UnitTesting;
using Calculate;
using System.Transactions;
using System.Text;
using System;
using System.Collections.Generic;
using System.IO.Pipelines;

namespace CalculateTests.Tests;

[TestClass]
public sealed class CalulateTests
{
    [TestMethod]
    public void Properties_SetAtConstruction()
    {
        //Arrange
        string? output1 = null;
        string? output2 = null;

        string nameInput = "Messi";

        var program = new Program
        {
            WriteLine = r =>
            {
                if (output1 == null)
                {
                    output1 = r;
                }
                else
                {
                    output2 = r;
                }
            },
            ReadLine = () => nameInput
        };

        //Act
        program.WriteLine("Enter name:");
        string? input = program.ReadLine();
        program.WriteLine($"Hey There, {input}");

        //Assert
        Assert.AreEqual<string>("Enter name:", output1);
        Assert.AreEqual<string>("Hey There, Messi", output2);
        Assert.AreEqual<string>("Messi", input);
    }


    [TestMethod]
    public void Add_ValidInput_ReturnsSum()
    {
        // Arrange
        double a = 5;
        double b = 3;

        // Act
        double result = Calculator.Add(a, b);

        // Assert
        Assert.AreEqual<double>(8, result);
    }

    [TestMethod]
    public void Subtract_ValidInput_ReturnsDifference()
    {
        // Arrange
        double a = 5;
        double b = 3;

        // Act
        double result = Calculator.Subtract(a, b);

        // Assert
        Assert.AreEqual<double>(2, result);
    }

    [TestMethod]
    public void Multiply_ValidInput_ReturnsProduct()
    {
        // Arrange
        double a = 67;
        double b = 7;

        // Act
        double result = Calculator.Multiply(a, b);

        // Assert
        Assert.AreEqual<double>(469, result);
    }

    [TestMethod]
    public void Divide_ValidInput_ReturnsQuotient()
    {
        // Arrange
        double a = 6;
        double b = 3;

        // Act
        double result = Calculator.Divide(a, b);

        // Assert
        Assert.AreEqual<double>(2, result);
    }

    [TestMethod]
    public void Divide_GetsDecimalQuotient()
    {
        // Arrange
        double a = 7;
        double b = 2;

        // Act
        double result = Calculator.Divide(a, b);

        // Assert
        Assert.AreEqual<double>(3.5, result);
    }

    [TestMethod]

    public void TryCalculate_Expression_ReturnsCorrectResult()
    {
        // Arrange
        string expression = "10 + 5";
        double expected = 15;

        // Act
        bool input = Calculator.TryCalculate(expression, out double result);

        // Assert
        Assert.IsTrue(input);
        Assert.AreEqual<double>(expected, result);
    }

    [TestMethod]
    public void TryCalculate_InvalidExpression_ReturnsFalse()
    {
        //Arrange
        string expression = "10 * 5";

        //Act
        bool input = Calculator.TryCalculate(expression, out double result);

        //Assert
        Assert.IsTrue(input);
        Assert.AreEqual<double>(50, result);
    }

    [TestMethod]
    public void TryCalculate_ExpressionMissingSpace_ReturnsFalse()
    {
        // Arrange
        string expression = "67+67";

        // Act
        bool input = Calculator.TryCalculate(expression, out double result);

        // Assert
        Assert.IsFalse(input);
    }

    [TestMethod]
    public void TryCalculate_DivideByZero_ReturnsFalse()
    {
        // Arrange
        string expression = "314762 / 0";

        // Act
        bool input = Calculator.TryCalculate(expression, out double result);

        // Assert
        Assert.IsFalse(input);
    }

    [DataTestMethod]
    [DataRow(6, '+', 7, 13)]
    [DataRow(21, '-', 3, 18)]
    [DataRow(100, '*', 14, 1400)]
    [DataRow(6, '/', 3, 2)]
    public void MathematicalOperations_ContainsAllOperations(double left, char op, double right, double expected)
    {
        // Arrange
        var operations = Calculator.MathematicalOperations;

        // Act & Assert
        Assert.IsTrue(operations.ContainsKey(op));

        var result = operations[op](left, right);

        Assert.AreEqual<double>(expected, result);
      
    }

    [TestMethod]
    public void Run_Completes_Successfully()
    {
        //Arrange
        String[] inputs =
        {
            "10 + 5",
            "20 - 4",
            "3 * 7",
            "16 / 2",
            ""
        };

        int Index = 0;
        var outputs = new List<string>();

        var program = new Program
        {
            ReadLine = () => inputs[Index++],
            WriteLine = s => outputs.Add(s)
        };

        //Act
        program.Run();

        //Assert
        Assert.AreEqual<string>("Result: 15", outputs[2]);
        Assert.AreEqual<string>("Result: 16", outputs[4]);
        Assert.AreEqual<string>("Result: 21", outputs[6]);
        Assert.AreEqual<string>("Result: 8", outputs[8]);
    }

    [TestMethod]
    public void Run_InvalidExpression_ReturnsPrompt()
    {
        //Arrange
        String[] inputs =
        {
            "10 / 0",
            "5 ++ 67",
            ""
        };

        int Index = 0;
        var outputs = new List<string>();

        var program = new Program
        {
            ReadLine = () => inputs[Index++],
            WriteLine = s => outputs.Add(s)
        };

        //Act
        program.Run();

        //Assert
        Assert.AreEqual<string>("Invalid expression. Please try again.", outputs[2]);
        Assert.AreEqual<string>("Invalid expression. Please try again.", outputs[4]);
        Assert.AreEqual<string>("Thanks 4 playin!", outputs[7]);
    }



}
