namespace Dag.Tests;


using Dag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;


/// <summary>
/// This class handles calc pgrm testing
/// </summary>
[TestClass]
public class CalculateTests
{
    private Calculator? _calculator;


    /// <summary>
    /// Initializes the test environment by creating a new instance of the Calculator class before each test method is executed. This ensures that each test starts with a fresh calculator instance, preventing any state carryover from previous tests.
    /// </summary>
    [TestInitialize]
    public void TestInit()
    {
        _calculator = new();
    }


    /// <summary>
    /// Tests addition operations performed
    /// </summary>
    [TestMethod]
    public void AddTest()
    {
        //Arrange
        const string testInput = "4 + 5";
        //Act
        bool valid = _calculator!.TryCalculate(testInput, out int result);

        //Assert
        Assert.AreEqual<int>(9, result);
        Assert.IsTrue(valid);
    }


    /// <summary>
    /// Tests subtractions operations
    /// </summary>
    [TestMethod]
    public void SubTest()
    {

        //Arrange
        const string testInput = "9 - 4";
        //Act
        bool valid = _calculator!.TryCalculate(testInput, out int result);
        //Assert
        Assert.AreEqual<int>(5, result);
        Assert.IsTrue(valid);
    }


    /// <summary>
    /// Tests multiplication operations
    /// </summary>
    [TestMethod]
    public void MultTests()
    {
        //Arrange
        const string testInput = "9 * 4";
        //Act
        bool valid = _calculator!.TryCalculate(testInput, out int result);
        //Assert
        Assert.AreEqual<int>(36, result);
        Assert.IsTrue(valid);
    }


    /// <summary>
    /// Tests division operations
    /// </summary>
    [TestMethod]
    public void DivideTest()
    {
        const string testInput = "24 / 3";
        bool valid = _calculator!.TryCalculate(testInput, out int result);
        Assert.AreEqual<int>(8, result);
        Assert.IsTrue(valid);
    }

    /// <summary>
    /// Tests division by zero exception
    /// </summary>
    [TestMethod]
    public void DivideByZeroExceptionTest()
    {
        Assert.Throws<DivideByZeroException>(() => Calculator.Divide(0, 0));
    }


    /// <summary>
    /// Tests invalid expressions and checks if the calculator returns false for them
    /// </summary>
    /// <param name="testInput"></param>
    [TestMethod]
    [DataRow("1 hello 3")]
    [DataRow("1 2")]
    [DataRow("1 / 0")]
    [DataRow(" ")]
    [DataRow("1 + four")]
    [DataRow("1 % 2")]
    public void InvalidExpressionTest(string testInput)
    {
        bool valid = _calculator!.TryCalculate(testInput!, out int _);
        Assert.IsFalse(valid);
    }

}
