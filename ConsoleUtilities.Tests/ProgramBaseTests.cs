namespace ConsoleUtilities.Tests;

[TestClass]
public sealed class ProgramBaseTests
{
    [TestMethod]
    public void WriteLineAndReadLine_SetAtConstruction_InvokesProvidedDelegates()
    {
        string? written = null;
        ProgramBase program = new()
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
        ProgramBase program = new();

        Assert.AreEqual<Type?>(typeof(Console), program.WriteLine.Method.DeclaringType);
        Assert.AreEqual<string>(nameof(Console.WriteLine), program.WriteLine.Method.Name);
    }

    [TestMethod]
    public void ReadLine_Default_InvokesConsoleReadLine()
    {
        ProgramBase program = new();

        Assert.AreEqual<Type?>(typeof(Console), program.ReadLine.Method.DeclaringType);
        Assert.AreEqual<string>(nameof(Console.ReadLine), program.ReadLine.Method.Name);
    }
}
