namespace Dag.Tests;

using Dag;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

/// <summary>
/// Verifies runtime behavior for the console Program loop.
/// </summary>
[TestClass]
public class ProgramRuntimeTests
{
    /// <summary>
    /// This test checks if the Program class throws ArgumentNullException when null delegates are passed to its constructor.
    /// </summary>
    [TestMethod]
    public void ProgramNullDelegatesThrowArgumentNullException()
    {
        var writeLineException = Assert.Throws<ArgumentNullException>(
            () => new Program(null!, Console.ReadLine));
        Assert.AreEqual<string?>("writeLine", writeLineException.ParamName);

        var readLineException = Assert.Throws<ArgumentNullException>(
            () => new Program(Console.WriteLine, null!));
        Assert.AreEqual<string?>("readLine", readLineException.ParamName);
    }

    /// <summary>
    /// This test checks if the Program class exits gracefully when the ReadLine delegate returns null.
    /// </summary>
    [TestMethod]
    public void ProgramNullInputExitsGracefully()
    {
        var output = new List<string>();
        var program = new Program(output.Add, () => null);

        program.Run();

        Assert.AreSequenceEqual(
            ["Enter a calculation in the form: number operator number", "Expression: ", "Goodbye."], output);
    }

    /// <summary>
    /// This test checks if the Program class correctly writes the result of a calculation to the output when a valid expression is provided.
    /// </summary>
    [TestMethod]
    public void ProgramWritesCalcToScreen()
    {
        var output = new List<string>();
        var input = new Queue<string?>(new string?[] { "2 + 2", null });
        var program = new Program(output.Add, input.Dequeue);

        program.Run();

        Assert.AreSequenceEqual(
            [
                "Enter a calculation in the form: number operator number",
                "Expression: ",
                "Result: 4",
                "Expression: ",
                "Goodbye."
            ], output);
    }

    /// <summary>
    /// This test launches the program and checks whether the Calculator object is instantiated.
    /// </summary>
    [TestMethod]
    public void CalculatorObjInitTest()
    {
        var program = new Program();
        Assert.IsNotNull(program.CalculatorInstance);
    }
}
