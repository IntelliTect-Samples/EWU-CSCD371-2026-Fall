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
}
