namespace Calculate.Tests;

[TestClass]
public class ProgramTests
{
    [TestMethod]
    public void WriteLineUsesCustomDelegate()
    {
        string? output = null;

        Program program = new()
        {
            WriteLine = message => output = message
        };

        program.WriteLine("Hello");

        Assert.AreEqual<string?>("Hello", output);
    }

    [TestMethod]
    public void ReadLineUsesCustomDelegate()
    {
        Program program = new()
        {
            ReadLine = () => "3 + 4"
        };

        string? input = program.ReadLine();

        Assert.AreEqual<string?>("3 + 4", input);
    }
}

