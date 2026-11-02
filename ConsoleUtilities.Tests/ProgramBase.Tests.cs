namespace ConsoleUtilities.Tests;

[TestClass]
public class ProgramBaseTests
{
    private sealed class TestProgram : ProgramBase { }

    [TestMethod]
    public void WriteLine_SetAtConstruction_InvokesProvidedDelegate()
    {
        string? written = null;
        TestProgram program = new() { WriteLine = text => written = text };

        program.WriteLine("Hello");

        Assert.AreEqual<string?>("Hello", written);
    }

    [TestMethod]
    public void ReadLine_SetAtConstruction_ReturnsProvidedValue()
    {
        TestProgram program = new() { ReadLine = () => "3 + 4" };

        string? input = program.ReadLine();

        Assert.AreEqual<string?>("3 + 4", input);
    }

    [TestMethod]
    public void WriteLine_Default_WritesToConsole()
    {
        TextWriter original = Console.Out;
        using StringWriter writer = new();
        try
        {
            Console.SetOut(writer);
            TestProgram program = new();

            program.WriteLine("Hello");

            Assert.AreEqual<string>($"Hello{Environment.NewLine}", writer.ToString());
        }
        finally
        {
            Console.SetOut(original);
        }
    }

    [TestMethod]
    public void ReadLine_Default_ReadsFromConsole()
    {
        TextReader original = Console.In;
        using StringReader reader = new("42 - 2");
        try
        {
            Console.SetIn(reader);
            TestProgram program = new();

            string? input = program.ReadLine();

            Assert.AreEqual<string?>("42 - 2", input);
        }
        finally
        {
            Console.SetIn(original);
        }
    }
}