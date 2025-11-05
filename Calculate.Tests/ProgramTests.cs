using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Calculate;
using System.Security;


namespace Calculate.Tests;

[TestClass]
public class ProgramTests
{
    private static readonly string?[] Quit = new[] { "q" };
    private static readonly string?[] QuitUpper = new[] { "Q" };
    private static readonly string?[] EofOnly = new string?[] { null };
    private static readonly string?[] EmptyThenQuit = new string?[] { "", "q" };

    private static readonly string?[] AddThenQuit = new[] { "3 + 4", "q" };
    private static readonly string?[] TwoOpsThenQuit = new[] { "3 + 4", "3 - 1", "q" };

    private static readonly string?[] NoSpacesThenQuit = new[] { "3+4", "q" };
    private static readonly string?[] UnknownOpThenQuit = new[] { "3 ^ 2", "q" };
    private static readonly string?[] DivZeroThenQuit = new[] { "1 / 0", "q" };

    [TestMethod]
    public void Properties_CanBeSetAtConstructionInvokedCorrectly_Success()
    {
        // Arrange
        var writes = new List<string?>();
        var inputs = new Queue<string?>(new[] { "hello", (string?)null });

        Action<string?> captureWrite = s => writes.Add(s);
        Func<string?> supplyRead = () => inputs.Count > 0 ? inputs.Dequeue() : null;

        // Act
        var program = new Program(captureWrite, supplyRead);

        program.WriteLine("first");
        var read1 = program.ReadLine();
        var read2 = program.ReadLine(); //will return null

        // Assert
        Assert.AreEqual<int>(1, writes.Count);
        Assert.AreEqual<string?>("first", writes[0]);
        Assert.AreEqual<string?>("hello", read1);
        Assert.AreEqual<string?>(null, read2);
    }

    [TestMethod]
    public void DefaultConstructor_WritesAndReadsConsoleBehavior_Success()
    {
        var originalIn = Console.In;
        var originalOut = Console.Out;
        try
        {
            // Arrange
            Console.SetIn(new StringReader("q" + Environment.NewLine));
            var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            var program = new Program();
            var exit = program.Run();

            // Assert
            Assert.AreEqual<int>(0, exit);
            StringAssert.Contains(sw.ToString(), "Enter an expression");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ThrowsWhenWriteLineIsNull_Success()
    {
        _ = new Program(null!, () => "input");
    }

    [TestMethod]
    [ExpectedException(typeof(ArgumentNullException))]
    public void Constructor_ThrowsWhenReadLineIsNull_Success()
    {
        _ = new Program(s => {}, null!);
    }

    [TestMethod]
    public void Run_ReturnsNonZeroWhenEndOfInputOccurs_Success()
    {
        // Arrange
        var linesWritten = new List<string?>();
        var inputs = new Queue<string?>(EofOnly);

        var program = new Program(linesWritten.Add, () => inputs.Dequeue());

        // Act
        var exitCode = program.Run();

        // Assert
        Assert.AreEqual<int>(1, exitCode);
    }

    [TestMethod]
    public void Run_QuitsWhenUserEntersQ_Success()
    {
        // Arrange
        var outputs = new List<string?>();
        var inputs = new Queue<string?>(Quit);

        var program = new Program(outputs.Add, () => inputs.Dequeue());

        // Act
        var exitCode = program.Run();

        // Assert
        Assert.AreEqual<int>(0, exitCode);
        Assert.IsTrue(outputs.Any(o => o!.Contains("Enter an expression", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Run_QuitUppercaseQ_Success()
    {
        // Arrange
        var outputs = new List<string?>();
        var inputs = new Queue<string?>(QuitUpper);
        var program = new Program(outputs.Add, () => inputs.Dequeue());

        // Act
        var exitCode = program.Run();

        // Assert
        Assert.AreEqual<int>(0, exitCode);
        Assert.IsTrue(outputs.Any(o => o!.Contains("Enter an expression", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Run_CalculatesValidExpression_Success()
    {
        // Arrange
        var outputs = new List<string?>();
        var inputs = new Queue<string?>(AddThenQuit);
        var program = new Program(outputs.Add, () => inputs.Dequeue());

        // Act

        var exitCode = program.Run();
        // Assert
        Assert.AreEqual<int>(0, exitCode);
        Assert.IsTrue(outputs.Any(o => o!.Contains("3 + 4 = 7", StringComparison.Ordinal)));
    }

    [TestMethod]
    public void Run_HandlesInvalidInput_Success()
    {
        // Arrange
        var outputs = new List<string?>();
        var inputs = new Queue<string?>(NoSpacesThenQuit);
        var program = new Program(outputs.Add, () => inputs.Dequeue());

        // Act
        var exitCode = program.Run();

        // Assert
        Assert.AreEqual<int>(0, exitCode);
        Assert.IsTrue(outputs.Any(o => o!.Contains("Invalid input", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Run_UnknownOperatorInput_HandlesGracefully()
    {
        // Arrange
        var outputs = new List<string?>();
        var inputs = new Queue<string?>(UnknownOpThenQuit);
        var program = new Program(outputs.Add, () => inputs.Dequeue());

        // Act
        var exitCode = program.Run();

        // Assert
        Assert.AreEqual<int>(0, exitCode);
        Assert.IsTrue(outputs.Any(o => o!.Contains("Invalid input", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Run_DivisionByZeroInput_HandlesGracefully()
    {
        // Arrange
        var outputs = new List<string?>();
        var inputs = new Queue<string?>(DivZeroThenQuit);
        var program = new Program(outputs.Add, () => inputs.Dequeue());

        // Act
        var exitCode = program.Run();

        // Assert
        Assert.AreEqual<int>(0, exitCode);
        Assert.IsTrue(outputs.Any(o => o!.Contains("Invalid input", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Run_ShowsPromptEachIteration_Success()
    {
        // Arrange
        var outputs = new List<string?>();
        var inputs = new Queue<string?>(TwoOpsThenQuit);
        var program = new Program(outputs.Add, () => inputs.Dequeue());

        // Act
        var exitCode = program.Run();

        // Assert
        Assert.AreEqual<int>(0, exitCode);
        int promptCount = outputs.Count(o => o != null && o!.Contains("Enter an expression", StringComparison.OrdinalIgnoreCase));
        Assert.AreEqual<int>(3, promptCount);
    }

    [TestMethod]
    public void Run_EmptyLineTreatedAsInvalidInput_Success()
    {
        // Arrange
        var outputs = new List<string?>();
        var inputs = new Queue<string?>(EmptyThenQuit);
        var program = new Program(outputs.Add, () => inputs.Dequeue());

        // Act
        var exitCode = program.Run();

        // Assert
        Assert.AreEqual<int>(0, exitCode);
        Assert.IsTrue(outputs.Any(o => o!.Contains("Invalid input", StringComparison.OrdinalIgnoreCase)));
    }

    [TestMethod]
    public void Main_EntryPointReadsAndRunsUntilQuit_Success()
    {
        // Arrange
        var originalIn = Console.In;
        var originalOut = Console.Out;
        try
        {
            Console.SetIn(new StringReader("q" + Environment.NewLine));
            var sw = new StringWriter();
            Console.SetOut(sw);

            // Act
            Program.Main();

            // Assert
            var stdout = sw.ToString();
            StringAssert.Contains(stdout, "Enter an expression");
        }
        finally
        {
            Console.SetIn(originalIn);
            Console.SetOut(originalOut);
        }
    }
}
