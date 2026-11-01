namespace Calculate.Tests;

[TestClass]
public sealed class ProgramTests
{
    [TestMethod]
    public void WriteLineAndReadLine_SetAtConstruction_InvokesProvidedDelegates()
    {
        string? written = null;
        Program program = new()
        {
            WriteLine = text => written = text,
            ReadLine = () => "input"
        };

        program.WriteLine("output");
        string? read = program.ReadLine();

        Assert.AreEqual<string?>("output", written);
        Assert.AreEqual<string?>("input", read);
    }

    [TestMethod]
    public void WriteLine_Default_InvokesConsoleWriteLine()
    {
        Program program = new();

        Assert.AreEqual<Type?>(typeof(Console), program.WriteLine.Method.DeclaringType);
        Assert.AreEqual<string>(nameof(Console.WriteLine), program.WriteLine.Method.Name);
    }

    [TestMethod]
    public void ReadLine_Default_InvokesConsoleReadLine()
    {
        Program program = new();

        Assert.AreEqual<Type?>(typeof(Console), program.ReadLine.Method.DeclaringType);
        Assert.AreEqual<string>(nameof(Console.ReadLine), program.ReadLine.Method.Name);
    }

    [TestMethod]
    public void Run_ValidAndInvalidInput_WritesResultsUntilBlankLine()
    {
        Queue<string?> inputs = new(["3 + 4", "bad", "10 / 2", ""]);
        List<string?> output = [];
        Program program = new()
        {
            WriteLine = output.Add,
            ReadLine = inputs.Dequeue
        };

        program.Run();

        CollectionAssert.AreEqual(new string?[] { "7", "Invalid calculation.", "5" }, output.Skip(1).ToArray());
    }

    [TestMethod]
    public void Run_EndOfInput_Exits()
    {
        List<string?> output = [];
        Program program = new()
        {
            WriteLine = output.Add,
            ReadLine = () => null
        };

        program.Run();

        Assert.AreEqual<int>(1, output.Count);
    }
}
